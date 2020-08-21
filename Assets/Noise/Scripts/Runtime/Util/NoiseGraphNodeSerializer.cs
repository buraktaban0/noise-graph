using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using UnityEngine;

namespace Noise.Runtime.Util
{
	public static class NoiseGraphNodeSerializer
	{

		public static NoiseGraphNodeData Serialize(NoiseGraphNode node)
		{
			var typeName = node.GetType().FullName;
			var json = JsonUtility.ToJson(node, true);

			return new NoiseGraphNodeData(typeName, json);
		}

		public static NoiseGraphNode Deserialize(NoiseGraphNodeData data)
		{

			NoiseGraphNode node;

			node = JsonUtility.FromJson(data.json, Type.GetType(data.typeName)) as NoiseGraphNode;

			if (node == null)
				throw new Exception($"Could not create node instance of type {data.typeName}.");


			return node;


		}

	}
}
