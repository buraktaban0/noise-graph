using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using Noise.Runtime.Util;
using UnityEngine;

[System.Serializable]
public class NoiseGraphNodeData
{

	public string typeName;

	public string json;

#if UNITY_EDITOR
	public Vector2 position;
#endif



	public NoiseGraphNodeData(string typeName, string json)
	{
		this.typeName = typeName;
		this.json = json;
	}

	public NoiseGraphNode Deserialize()
	{
		return NoiseGraphNodeSerializer.Deserialize(this);
	}

}

