using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using Noise.Runtime.Nodes;
using UnityEngine;

namespace Noise.Editor
{

	[System.Serializable]
	public class SerializedNodeWrapper : ScriptableObject
	{

		[SerializeReference]
		public NoiseGraphNode node;

	}

}
