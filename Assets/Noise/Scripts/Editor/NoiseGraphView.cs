using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noise.Editor.Serialization;
using Noise.Editor.Util;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor
{
	public class NoiseGraphView : GraphView
	{
		protected override bool canCopySelection => canDeleteSelection;

		protected override bool canCutSelection       => canDeleteSelection;
		protected override bool canPaste              => true;
		protected override bool canDuplicateSelection => canDeleteSelection;

		protected override bool canDeleteSelection
		{
			get
			{
				return !selection.Any(item => item is NoiseGraphNodeView view && view.NodeType == typeof(MasterNode));
			}
		}

		internal NoiseGraphWindow window;

		private NoiseGraph graph;

		private SearchWindowProvider searchWindowProvider;

		private EdgeConnectorListener edgeConnectorListener;

		public NoiseGraphView(string name, NoiseGraphWindow window)
		{
			this.name = name;
			this.window = window;

			graph = NoiseGraphWindow.currentViewedGraph;

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new ClickSelector());

			AddSearchWindow();

			edgeConnectorListener = new EdgeConnectorListener(this, searchWindowProvider);

			this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			styleSheets.Add(Resources.Load<StyleSheet>("NoiseGraph"));

			var grid = new GridBackground();
			Insert(0, grid);
			grid.StretchToParentSize();

			this.AddNode<MasterNode>(new Vector2(400, 400));

			deleteSelection += DeleteSelection;

			serializeGraphElements += SerializeElements;
			unserializeAndPaste += UnserializeAndPasteElements;

			//var masterView = NoiseGraphNodeView.Build<MasterNode>(this, searchWindowProvider);
			//masterView.SetPosition(new Rect(400, 400, 0, 0));

			//this.AddElement(masterView);

			SerializeAndSaveGraph();
		}

		private string SerializeElements(IEnumerable<GraphElement> elements)
		{
			return Serializer.SerializeElements(elements).ToJson();
		}


		private void UnserializeAndPasteElements(string operationName, string data)
		{
			Debug.Log($"Uns {operationName}");
			IEnumerable<GraphElement> elements =
				Serializer.DeserializeElements(data, this, searchWindowProvider, edgeConnectorListener);

			foreach (var graphElement in elements)
			{
				this.AddElement(graphElement);
				if (graphElement is NoiseGraphNodeView nodeView)
				{
					nodeView.ResetGuid();
				}
			}

			OnGraphModified();
		}

		private void SerializeAndSaveGraph()
		{
			var serializedGraph = Serializer.SerializeGraph(this);
			this.graph.SetSerializedState(serializedGraph);
			this.graph.DeserializeNodes();

			EditorUtility.SetDirty(this.graph);
		}

		private void RefreshGraphState()
		{
			graph.ProcessAllNodes();

			ports.ForEach(port =>
			{
				var node = graph.GetNode(((NoiseGraphNodeView) port.node).GUID);
				var value = node.GetInputOrOutput(port.name);

				var typeClass = port.GetClasses().First(c => c.Contains("type"));
				port.RemoveFromClassList(typeClass);

				Type type = null;

				if (value == null)
				{
					type = port.portType;
				}
				else
				{
					type = value.GetType();
				}

				port.AddToClassList($"type{type.Name}");

				port.tooltip = type.Name;
				port.parent.tooltip = type.Name;

				port.MarkDirtyRepaint();
			});

			edges.ForEach(edge => { edge.MarkDirtyRepaint(); });

			MarkDirtyRepaint();
		}

		private void DeleteSelection(string operationName, AskUser askUser)
		{
			for (int i = selection.Count; i >= 0; i--)
			{
				if (i >= selection.Count)
				{
					continue;
				}

				var item = selection[i];
				switch (item)
				{
					case NoiseGraphNodeView nodeView:
						nodeView.OnDeleted();
						break;
					case Edge edge:
						edge.input.Disconnect(edge);
						edge.output.Disconnect(edge);
						edge.RemoveFromHierarchy();
						break;
				}
			}

			OnGraphModified();
		}


		private void AddSearchWindow()
		{
			searchWindowProvider = SearchWindowProvider.Build(window, this);
			nodeCreationRequest += context =>
			{
				searchWindowProvider.ConnectedPort = null;
				SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
			};
		}

		public void AddNode<T>(Vector2 pos) where T : NoiseGraphNode => AddNode(typeof(T), pos);

		public NoiseGraphNodeView AddNode(Type t, Vector2 pos)
		{
			var nodeInstance = Activator.CreateInstance(t) as NoiseGraphNode;
			var view = new NoiseGraphNodeView(nodeInstance, this, searchWindowProvider, edgeConnectorListener);
			this.AddElement(view);
			view.SetPosition(new Rect(pos, Vector2.zero));

			OnGraphModified();

			return view;
		}

		public void AddEdge(Edge edge)
		{
			OnGraphModified();
		}

		public void ConnectPorts(Port connectedPort, Port port)
		{
			var edge = port.ConnectTo(connectedPort);

			this.AddElement(edge);

			OnGraphModified();
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();

			var startPortData = (PortData) (startPort.userData);
			var startPortTypes = startPortData.types;

			ports.ForEach(port =>
			{
				var portData = (PortData) port.userData;
				var portTypes = portData.types;

				if (port.direction == startPort.direction ||
				    port == startPort ||
				    port.node == startPort.node)
				{
					//Debug.Log($"Simple fail {startPort.name} {port.name}");
					return;
				}

				if (portTypes.Intersect(startPortTypes).Any())
				{
					//Debug.Log($"Intersect {startPort.name} {port.name}");
					compatiblePorts.Add(port);
					return;
				}


				var outputTypes = port.direction == Direction.Output ? portTypes : startPortTypes;
				var inputTypes = port.direction == Direction.Output ? startPortTypes : portTypes;

				var implicitCompatibility = outputTypes.Any(t1 => inputTypes.Any(t2 =>
				{
					var hasMethod = t2.GetMethods(BindingFlags.Public | BindingFlags.Static)
					                  .FirstOrDefault(m => m.Name.Equals("op_Implicit") &&
					                                       m.GetParameters()
					                                        .FirstOrDefault(p => p.ParameterType == t1) != default &&
					                                       m.ReturnType == t2) != null;
					//Debug.Log($"{t1}  {t2}  {hasMethod} - {port.name}");
					return hasMethod;
				}));

				if (implicitCompatibility)
				{
					//Debug.Log($"Implicit {startPort.name} {port.name}");
					compatiblePorts.Add(port);
				}
			});

			return compatiblePorts;
		}

		public void OnGraphModified()
		{
			SerializeAndSaveGraph();
			RefreshGraphState();
			graph.ProcessAllNodes();
		}
	}
}
