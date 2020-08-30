using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Noise.Runtime.Nodes;
using UnityEngine;

namespace Noise.Runtime.Serialization
{
	public static partial class Serializer
	{
		public static SerializedNode SerializeNode(NoiseGraphNode node)
		{
			return new SerializedNode(node.GetType().FullName,
			                          JsonUtility.ToJson(node, true));
		}

		public static NoiseGraphNode DeserializeNode(SerializedNode serializedNode)
		{
			var type = Type.GetType(serializedNode.typeName);
			var nodeObj = JsonUtility.FromJson(serializedNode.json, type);
			var node = nodeObj as NoiseGraphNode;
			return node;
		}
	}
}
