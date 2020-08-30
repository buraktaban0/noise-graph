using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Noise.Runtime;
using Noise.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using Noise.Editor.Util;
using Noise.Runtime.Nodes;
using System.Linq.Expressions;
using Noise.Editor.Elements;
using Procedural.GPU;
using Unity.Mathematics;
using Noise.Runtime.Util;
using System.ComponentModel;

namespace Noise.Editor
{
	public class NoiseGraphNodeView : Node
	{
		private static readonly Type InputAttrType = typeof(InputAttribute);
		private static readonly Type OutputAttrType = typeof(OutputAttribute);

		private const float MAXPreviewSize = 150;

		public int GUID => node.GUID;

		public Port[] Ports    => this.Query<Port>().ToList().ToArray();
		public Type   NodeType => node.GetType();

		public NoiseGraph graph => NoiseGraphWindow.currentViewedGraph;

		public NoiseGraphView graphView;

		public SearchWindowProvider searchWindowProvider;

		public EdgeConnectorListener edgeConnectorListener;

		public NoiseGraphNode node;

		private SerializedNodeWrapper serializedNodeWrapper;
		private SerializedObject serializedNode;

		private VisualElement previewElement;

		public NoiseGraphNodeView(NoiseGraphNode node, NoiseGraphView graphView,
		                          SearchWindowProvider searchWindowProvider,
		                          EdgeConnectorListener edgeConnectorListener)
		{
			this.node = node;
			this.graphView = graphView;
			this.searchWindowProvider = searchWindowProvider;
			this.edgeConnectorListener = edgeConnectorListener;


			this.title = node.GetNodeName();

			serializedNodeWrapper = ScriptableObject.CreateInstance<SerializedNodeWrapper>();

			serializedNodeWrapper.node = this.node;
			serializedNode = new SerializedObject(serializedNodeWrapper);

			serializedNode.Update();


			var ss = Resources.Load<StyleSheet>("NoiseGraph");

			styleSheets.Add(ss);
			inputContainer.styleSheets.Add(ss);
			outputContainer.styleSheets.Add(ss);

			inputContainer.AddToClassList("input");
			outputContainer.AddToClassList("output");

			BuildLayout();
		}


		public void BuildLayout()
		{
			var nodeType = node.GetType();

			var publicFields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

			var inputFields = publicFields.Where(f => f.IsDefined(InputAttrType, true)).ToList();
			var outputFields = publicFields.Where(f => f.IsDefined(OutputAttrType, true)).ToList();


			foreach (var f in inputFields)
			{
				var inputAttr = f.GetCustomAttribute<InputAttribute>();
				var allowedTypes = inputAttr.AdditionalAllowedTypes.Append(f.FieldType).Distinct().ToArray();
				SetupField(f, Direction.Input, inputAttr.HasPort, inputAttr.HasInlineEditor, allowedTypes);
			}

			foreach (var f in outputFields)
			{
				var allowedTypes = new Type[] {f.FieldType};
				SetupField(f, Direction.Output, hasPort: true, hasInlineEditor: false, allowedTypes);
			}

			RefreshExpandedState();
			RefreshPorts();
		}


		private void SetupField(FieldInfo f, Direction direction, bool hasPort, bool hasInlineEditor,
		                        Type[] allowedTypes)
		{
			if (!hasPort && !hasInlineEditor && f.IsDefined(typeof(NodeIgnoreAttribute)) == false)
			{
				Debug.LogWarning(
					$"Field '{f.Name}' of type '{node.GetType().FullName}' has neither a port nor an inline editor. Ignoring field. Consider using 'NodeIgnore' attribute.");
				return;
			}

			var container = direction == Direction.Input ? inputContainer : outputContainer;

			var row = new VisualElement();

			Port port = null;
			port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Single, f.FieldType);
			port.name = f.Name;
			port.portName = f.Name;
			port.tooltip = f.FieldType.Name;

			port.userData = new PortData
			{
				types = allowedTypes
			};

			port.AddManipulator(new EdgeConnector<Edge>(edgeConnectorListener));

			if (direction == Direction.Output)
			{
				port.Q<Label>().tooltip = f.FieldType.Name;
				row.Add(port);
				container.Add(row);
				SetupFieldForPreview(f, row);
				return;
			}

			if (hasPort)
			{
				SetupFieldForPreview(f, row);
			}
			else
			{
				port.AddToClassList("dummyPort");
				row.AddToClassList("portlessRow");
			}

			var lbl = port.Q<Label>();
			lbl.text = "";
			lbl.style.marginLeft = 0;
			lbl.style.marginRight = 0;

			NodeIMGUIContainer boundElement = null;
			boundElement = new NodeIMGUIContainer(f, serializedNode, serializedNodeWrapper, this.node, OnNodeModified)
			{
				//tooltip = f.FieldType.Name,
				labelOnly = !hasInlineEditor
			};

			row.Add(port);
			row.Add(boundElement);

			container.Add(row);
		}

		private void SetupFieldForPreview(FieldInfo f, VisualElement row)
		{
			Vector2 GetPreviewImagePosition(Vector2 mouseWorldPos, Vector2 elementSize)
			{
				return this.parent.WorldToLocal(mouseWorldPos) + elementSize.y * Vector2.down +
				       new Vector2(1f, -1f) * 2f;
			}

			row.RegisterCallback<MouseEnterEvent>(ev =>
			{
				graph.ProcessAllNodes();

				previewElement?.RemoveFromHierarchy();

				var graphNode = graph.GetNode(this.GUID);

				var previewedValue = graphNode.GetInputOrOutput(f.Name);

				if (previewedValue == null)
				{
					previewElement = null;
					return;
				}

				previewElement = PreviewUtil.GetPreviewElement(previewedValue, MAXPreviewSize);

				//previewElement.style.width = previewSize;
				//previewElement.style.height = previewSize;


				this.parent.Add(previewElement);

				previewElement.style.position = Position.Absolute;

				var mousePosition = ev.mousePosition;

				previewElement.RegisterCallback<GeometryChangedEvent>(geoEv =>
				{
					var elementSize = previewElement.layout.size;
					previewElement.transform.position = GetPreviewImagePosition(mousePosition, elementSize);
				});
			});

			row.RegisterCallback<MouseMoveEvent>(ev =>
			{
				if (previewElement != null)
				{
					var elementSize = previewElement.layout.size;

					previewElement.transform.position = GetPreviewImagePosition(ev.mousePosition, elementSize);
				}
			});


			row.RegisterCallback<MouseLeaveEvent>(ev =>
			{
				if (previewElement == null)
					return;
				previewElement.RemoveFromHierarchy();
				previewElement = null;
			});
		}

		public static NoiseGraphNodeView Build<T>(NoiseGraphView graphView, SearchWindowProvider searchWindowProvider,
		                                          EdgeConnectorListener edgeConnectorListener)
			where T : NoiseGraphNode, new()
		{
			var node = new T();

			var view = new NoiseGraphNodeView(node, graphView, searchWindowProvider, edgeConnectorListener);

			return view;
		}

		private void OnNodeModified()
		{
			graphView.OnGraphModified();
		}

		public void OnDeleted()
		{
			var edges = Ports.Where(p => p.connected).SelectMany(p => p.connections).ToList();

			foreach (var edge in edges)
			{
				edge.input.Disconnect(edge);
				edge.output.Disconnect(edge);
				edge.RemoveFromHierarchy();
			}

			this.RemoveFromHierarchy();
		}

		public void ResetGuid()
		{
			node.GUID = new System.Random().Next();
		}
	}
}
