using Noise.Runtime.Attributes;
using Noise.Runtime.Util;
using Procedural.GPU;
using UnityEngine;

namespace Noise.Runtime.Nodes
{
	[NodeAttribute(Name = "Subtract")]
	public class SubtractNode : NoiseGraphNode
	{
		[Input(typeof(GPUBufferHandle))]
		public float A;

		[Input(typeof(GPUBufferHandle))]
		public float B;

		[Output]
		public float Out;


		public override void Process()
		{
			base.Process();

			var a = GetInput("A", this.A);
			var b = GetInput("B", this.B);

			if (a is float af && b is float bf)
			{
				SetOutput("Out", af - bf);
				return;
			}

			var buffA = GPUBufferHandle.Create(a);
			var buffB = GPUBufferHandle.Create(b);

			var material = MaterialCache.GetMaterial(MaterialCache.Op.Subtract);
			int pass = 0;

			material.SetTexture(HASH_TEXA, buffA);
			material.SetTexture(HASH_TEXB, buffB);

			Graphics.Blit(null, buffer, material, pass);

			buffA.ReleaseIfTemp();
			buffB.ReleaseIfTemp();

			SetOutput("Out", buffer);
		}
	}
}
