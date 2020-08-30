using System.Collections.Generic;
using System.Linq;
using Noise.Runtime.Serialization;
using UnityEditor.Experimental.GraphView;

namespace Noise.Editor.Serialization
{
	public static class Serializer
	{
		public static SerializedGraph SerializeGraph(GraphView graphView) =>
			SerializeElements(graphView.graphElements.ToList());


		public static SerializedGraph SerializeElements(IEnumerable<GraphElement> elements)
		{
			var serializedNodes = elements.OfType<NoiseGraphNodeView>()
			                              .Select(view => Runtime.Serialization.Serializer.SerializeNode(
				                                      view.node)).ToList();

			var serializedLinks = elements.OfType<Edge>()
			                              .Select(edge => SerializeEdge(edge))
			                              .ToList();

			return new SerializedGraph(serializedNodes, serializedLinks);
		}

		public static IEnumerable<GraphElement> DeserializeElements(string data, NoiseGraphView graphView,
		                                                            SearchWindowProvider searchWindowProvider,
		                                                            EdgeConnectorListener edgeConnectorListener)
		{
			return DeserializeElements(SerializedGraph.FromJson(data), graphView, searchWindowProvider,
			                           edgeConnectorListener);
		}

		public static IEnumerable<GraphElement> DeserializeElements(SerializedGraph serializedGraph,
		                                                            NoiseGraphView graphView,
		                                                            SearchWindowProvider searchWindowProvider,
		                                                            EdgeConnectorListener edgeConnectorListener)
		{
			var serializedNodes = serializedGraph.serializedNodes;
			var serializedLinks = serializedGraph.serializedLinks;

			var nodes = serializedNodes.Select(Runtime.Serialization.Serializer.DeserializeNode);
			var nodeViews = nodes.Select(node => new NoiseGraphNodeView(
				                             node, graphView, searchWindowProvider, edgeConnectorListener));

			var edges = serializedLinks.Select(link =>
			{
				var node0 = nodeViews.First(n => n.GUID == link.guid0);
				var node1 = nodeViews.First(n => n.GUID == link.guid1);

				var port0 = node0.Ports.First(p => p.name == link.field0);
				var port1 = node0.Ports.First(p => p.name == link.field1);

				var edge = port0.ConnectTo(port1);

				return edge;
			});

			var graphElements = nodeViews.OfType<GraphElement>().Concat(edges);
			return graphElements;
		}

		private static SerializedLink SerializeEdge(Edge edge)
		{
			var guid0 = ((NoiseGraphNodeView) edge.output.node).GUID;
			var field0 = edge.output.name;
			var guid1 = ((NoiseGraphNodeView) edge.input.node).GUID;
			var field1 = edge.input.name;

			return new SerializedLink(guid0, field0, guid1, field1);
		}
	}
}
