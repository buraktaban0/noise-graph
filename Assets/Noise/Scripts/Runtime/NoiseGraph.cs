using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using Noise.Runtime.Serialization;
using UnityEngine;

namespace Noise.Runtime
{
	[CreateAssetMenu(menuName = "Noise Graph", fileName = "Noise Graph")]
	public class NoiseGraph : ScriptableObject
	{
		public SerializedGraph serializedGraph;

		private List<NoiseGraphNode> nodes;

		private Dictionary<int, NoiseGraphNode> nodesByGUID;
		private Dictionary<int, List<SerializedLink>> linksByGUID;

		public List<NoiseGraphNode> DeserializeNodes()
		{
			this.nodes = serializedGraph?.serializedNodes.Select(Serializer.DeserializeNode).ToList();
			return this.nodes;
		}

		public NoiseGraphNode GetNode(int guid)
		{
			return nodes.First(node => node.GUID == guid);
		}

		public void ProcessAllNodes()
		{
			if (nodes == null || nodes.Count < 1)
			{
				nodes = DeserializeNodes();

				if (nodes == null)
					return;
			}

			nodes.ForEach(n => n.Clear());

			nodes.ForEach(EvaluateNode);
		}

		public void ProcessMainLine()
		{
			if (nodes == null || nodes.Count < 1)
			{
				nodes = DeserializeNodes();
			}

			var masterNode = nodes.First(n => n is MasterNode) as MasterNode;

			nodes.ForEach(n => n.Clear());

			EvaluateNode(masterNode);
		}


		public void EvaluateNode(NoiseGraphNode node)
		{
			if (node.wasProcessed)
			{
				return;
			}

			nodesByGUID = nodes.ToDictionary(n => n.GUID);
			linksByGUID = serializedGraph.serializedLinks.GroupBy(l => l.guid1).Select(g => g.ToList())
			                             .ToDictionary(list => list[0].guid1);

			if (linksByGUID.TryGetValue(node.GUID, out List<SerializedLink> inputLinks))
			{
				for (int i = 0; i < inputLinks.Count; i++)
				{
					var link = inputLinks[i];
					var inputNode = nodesByGUID[link.guid0];

					EvaluateNode(inputNode);

					var value = inputNode.GetOutput(link.field0, null);
					if (value != null)
						node.SetInput(link.field1, value);
				}
			}

			node.Process();
		}

		public void SetSerializedState(SerializedGraph serializedGraph)
		{
			this.serializedGraph = serializedGraph;
			this.nodes = DeserializeNodes();
		}
	}
}
