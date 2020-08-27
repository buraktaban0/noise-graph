using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor
{
	public class NoiseGraphView : GraphView
	{
		protected override bool canCopySelection      => true;
		protected override bool canCutSelection       => true;
		protected override bool canPaste              => true;
		protected override bool canDuplicateSelection => true;
		protected override bool canDeleteSelection    => true;

		private NoiseGraph graph;

		public NoiseGraphView()
		{
			graph = NoiseGraphWindow.currentViewedGraph;

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			styleSheets.Add(Resources.Load<StyleSheet>("NoiseGraph"));

			var grid = new GridBackground();
			Insert(0, grid);
			grid.StretchToParentSize();

			var masterView = NoiseGraphNodeView.Build<MasterNode>(this);
			masterView.SetPosition(new Rect(400, 400, 0, 0));

			this.AddElement(masterView);


			nodeCreationRequest += OnNodeCreationRequest;
		}

		private void OnNodeCreationRequest(NodeCreationContext obj)
		{
			Debug.Log("OnNodeCreationRequest");
			
			SearchWindowContext cxt = new SearchWindowContext(obj.screenMousePosition);
			var provider = new SearchWindowProvider();
			SearchWindow.Open(customStyle, provider)
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
					Debug.Log($"Simple fail {startPort.name} {port.name}");
					return;
				}

				if (portTypes.Intersect(startPortTypes).Any())
				{
					Debug.Log($"Intersect {startPort.name} {port.name}");
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
					Debug.Log($"{t1}  {t2}  {hasMethod} - {port.name}");
					return hasMethod;
				}));

				if (implicitCompatibility)
				{
					Debug.Log($"Implicit {startPort.name} {port.name}");
					compatiblePorts.Add(port);
				}
			});

			return compatiblePorts;
		}


		protected override void CollectCopyableGraphElements(IEnumerable<GraphElement> elements,
		                                                     HashSet<GraphElement> elementsToCopySet)
		{
			elements.ToList().ForEach(el =>
			{
				if (el is NoiseGraphNodeView view && !(view.node is MasterNode))
				{
					elementsToCopySet.Add(el);
				}
			});
		}
	}
}
