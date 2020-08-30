using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using Noise.Runtime.Util;
using UnityEngine;

namespace Noise.Runtime.Serialization
{
	[System.Serializable]
	public class SerializedNode
	{
		public string typeName;

		public string json;

#if UNITY_EDITOR
		public Vector2 position;
#endif


		public SerializedNode(string typeName, string json)
		{
			this.typeName = typeName;
			this.json = json;
		}

		
	}
}
