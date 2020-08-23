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

		private static readonly float previewSize = 150;

		public int GUID => node.GUID;

		public VisualElement inputPortsContainer => inputContainer[0];
		public VisualElement inputFieldsContainer => inputContainer[1];

		public VisualElement outputPortsContainer => outputContainer[1];
		public VisualElement outputFieldsContainer => outputContainer[0];

		public NoiseGraph graph;

		public NoiseGraphView graphView;

		public NoiseGraphNode node;

		private SerializedNodeWrapper serializedNodeWrapper;
		private SerializedObject serializedNode;

		private VisualElement previewElement;

		public NoiseGraphNodeView(NoiseGraphNode node, NoiseGraphView graphView)
		{
			this.node = node;
			this.graphView = graphView;

			graph = NoiseGraphWindow.currentViewedGraph;

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
				var allowedTypes = new Type[] { f.FieldType };
				SetupField(f, Direction.Output, hasPort: true, hasInlineEditor: false, allowedTypes);
			}

			RefreshExpandedState();
			RefreshPorts();

		}




		private void SetupField(FieldInfo f, Direction direction, bool hasPort, bool hasInlineEditor, Type[] allowedTypes)
		{

			if (!hasPort && !hasInlineEditor && f.IsDefined(typeof(NodeIgnoreAttribute)) == false)
			{
				Debug.LogWarning($"Field '{f.Name}' of type '{node.GetType().FullName}' has neither a port nor an inline editor. Ignoring field. Consider using 'NodeIgnore' attribute.");
				return;
			}

			var container = direction == Direction.Input ? inputContainer : outputContainer;

			var row = new VisualElement();

			//row.tooltip = f.FieldType.Name;

			//if (f.FieldType == typeof(GPUNoiseBufferHandle))
			//{
			//	SetupFieldForPreview(f, row);
			//}

			Port port = null;
			port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Single, f.FieldType);
			port.name = f.Name;
			port.portName = f.Name;
			port.tooltip = f.FieldType.Name;

			port.userData = new PortData
			{
				types = allowedTypes
			};

			if (direction == Direction.Output)
			{
				port.Q<Label>().tooltip = f.FieldType.Name;
				row.Add(port);
				container.Add(row);
				return;
			}

			if (!hasPort)
			{
				port.AddToClassList("dummyPort");
				row.AddToClassList("portlessRow");
			}

			var lbl = port.Q<Label>();
			lbl.text = "";
			lbl.style.marginLeft = 0;
			lbl.style.marginRight = 0;

			NodeIMGUIContainer boundElement = null;
			boundElement = new NodeIMGUIContainer(f, serializedNode, serializedNodeWrapper, this.node, OnNodeModified);
			boundElement.tooltip = f.FieldType.Name;

			boundElement.labelOnly = !hasInlineEditor;

			row.Add(port);
			row.Add(boundElement);

			container.Add(row);
		}

		private void SetupFieldForPreview(FieldInfo f, VisualElement row)
		{
			Vector2 GetPreviewImagePosition(Vector2 mouseWorldPos)
			{
				return this.parent.WorldToLocal(mouseWorldPos) + previewSize * Vector2.down + new Vector2(1f, -1f) * 2f;
			}

			row.RegisterCallback<MouseEnterEvent>(ev =>
			{

				if (previewElement != null)
				{
					previewElement.RemoveFromHierarchy();
				}

				var imageObj = f.GetValue(this.node);

				if (imageObj == null)
				{
					return;
				}

				previewElement = new Image()
				{
					image = (GPUNoiseBufferHandle)imageObj
				};

				previewElement.style.width = previewSize;
				previewElement.style.height = previewSize;

				this.parent.Add(previewElement);
				previewElement.style.position = Position.Absolute;

				previewElement.transform.position = GetPreviewImagePosition(ev.mousePosition);

			});

			row.RegisterCallback<MouseMoveEvent>(ev =>
			{
				if (previewElement != null)
				{
					previewElement.transform.position = GetPreviewImagePosition(ev.mousePosition);
				}
			});

			row.RegisterCallback<MouseLeaveEvent>(ev =>
			{
				if (previewElement != null)
				{
					previewElement.RemoveFromHierarchy();
					previewElement = null;
				}
			});
		}

		public static NoiseGraphNodeView Build<T>(NoiseGraphView graphView) where T : NoiseGraphNode, new()
		{
			var node = new T();

			var view = new NoiseGraphNodeView(node, graphView);

			return view;
		}

		private void OnNodeModified()
		{

		}

	}


	public struct PortData
	{
		public Type[] types;
	}


}
