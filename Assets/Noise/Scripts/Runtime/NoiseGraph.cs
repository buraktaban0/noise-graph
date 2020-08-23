using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using UnityEngine;

namespace Noise.Runtime
{

	[CreateAssetMenu(menuName = "Noise Graph", fileName = "Noise Graph")]
	public class NoiseGraph : ScriptableObject
	{

		public List<NoiseGraphNodeData> nodeData;
		public List<NoiseGraphLinkData> linkData;

		private List<NoiseGraphNode> nodes;

		private Dictionary<int, NoiseGraphNode> nodesByGUID;
		private Dictionary<int, List<NoiseGraphLinkData>> linksByGUID;

		public List<NoiseGraphNode> DeserializeNodes()
		{
			var nodes = nodeData.Select(data => data.Deserialize()).ToList();
			return nodes;
		}

		public void ForceProcessAll()
		{
			nodes.ForEach(n => n.Clear());

			nodes.ForEach(n => EvaluateNode(n));
		}

		public void Process()
		{
			if (nodes == null || nodes.Count < 1)
			{
				nodes = DeserializeNodes();
			}

			nodesByGUID = nodes.ToDictionary(n => n.GUID);
			linksByGUID = linkData.GroupBy(l => l.guid1).Select(g => g.ToList()).ToDictionary(list => list[0].guid1);

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

			List<NoiseGraphLinkData> inputLinks;
			if (linksByGUID.TryGetValue(node.GUID, out inputLinks))
			{
				for (int i = 0; i < inputLinks.Count; i++)
				{
					var link = inputLinks[i];
					var inputNode = nodesByGUID[link.guid0];

					EvaluateNode(inputNode);

					var f0 = inputNode.GetType().GetField(link.field0);
					var value = f0.GetValue(inputNode);
					node.SetInput(link.field1, value);

				}
			}

			node.Process();
		}


	}
}
