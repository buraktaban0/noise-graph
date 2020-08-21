using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Runtime.Nodes
{

	[System.Serializable]
	public class TestNode : NoiseGraphNode, IBinding
	{

		[Input(true)]
		[Min(0.1f)]
		public float Quality = 3;

		[Input]
		public Vector3 Position = Vector3.up;

		[Input]
		public float Power = 2f;

		[Output]
		public Vector4 Matrix = Vector4.zero;




	}
}
