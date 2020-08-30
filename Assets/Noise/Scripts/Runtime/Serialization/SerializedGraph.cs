using System.Collections.Generic;
using UnityEngine;

namespace Noise.Runtime.Serialization
{
	[System.Serializable]
	public class SerializedGraph
	{
		public List<SerializedNode> serializedNodes;
		public List<SerializedLink> serializedLinks;

		public SerializedGraph(List<SerializedNode> serializedNodes, List<SerializedLink> serializedLinks)
		{
			this.serializedNodes = serializedNodes;
			this.serializedLinks = serializedLinks;
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(this, true);
		}

		public static SerializedGraph FromJson(string json)
		{
			return JsonUtility.FromJson<SerializedGraph>(json);
		}
	}
}
