using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	[CreateAssetMenu(menuName = "Noise/Noise Graph")]
	public class NoiseGraph : NodeGraph
	{

		public NoiseMasterNode MasterNode => this.nodes.FirstOrDefault(n => n is NoiseMasterNode) as NoiseMasterNode;

		public int seed = 0;

		public int width;
		public int height;
		
		public int BufferLength => width * height;

		public override Node AddNode(Type type)
		{
			Node node = base.AddNode(type);

			UpdateAllNodes();

			return node;
		}


		public void UpdateAllNodes()
		{
			var masterNode = MasterNode;

			if (masterNode != null)
			{
				masterNode.GetValue(null);
			}

		}

		public override void OnNodeModified(Node node)
		{
			base.OnNodeModified(node);

			UpdateAllNodes();
		}

	}

}
