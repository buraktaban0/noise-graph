using Noise.Runtime.Attributes;
using Noise.Runtime.Util;
using Procedural.GPU;
using UnityEngine;

namespace Noise.Runtime.Nodes
{
	[NodeAttribute(Name = "Perlin")]
	public class PerlinNode : NoiseGraphNode
	{
		public float Amplitude = 1.0f;

		[Output]
		public GPUBufferHandle Out;


		public override void Process()
		{
			base.Process();

			var amplitude = GetInput("A", this.Amplitude);

			SetOutput("Out", buffer);
		}
	}
}
