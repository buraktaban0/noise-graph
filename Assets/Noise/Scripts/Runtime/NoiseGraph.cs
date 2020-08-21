using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using UnityEngine;

namespace Noise.Runtime
{

	[CreateAssetMenu(menuName = "Noise Graph", fileName = "Noise Graph")]
	public class NoiseGraph : ScriptableObject
	{

		public List<NoiseGraphNodeData> nodeData;
		public List<NoiseGraphLinkData> linkData;

		public List<NoiseGraphNode> DeserializeNodes()
		{
			var nodes = nodeData.Select(data => data.Deserialize()).ToList();
			return nodes;
		}
	}
}
