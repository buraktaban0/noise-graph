using Noise.Runtime.Attributes;
using Noise.Runtime.Util;
using Procedural.GPU;
using Unity.Mathematics;
using UnityEngine;

namespace Noise.Runtime.Nodes
{
	[NodeAttribute(Name = "Lerp")]
	public class LerpNode : NoiseGraphNode
	{
		[Input(typeof(GPUBufferHandle))]
		public float A;

		[Input(typeof(GPUBufferHandle))]
		public float B;

		[Input(typeof(GPUBufferHandle))]
		[Range(0f, 1f)]
		public float t;

		[Output]
		public float Out;


		public override void Process()
		{
			base.Process();

			var a = GetInput("A", this.A);
			var b = GetInput("B", this.B);
			var t = GetInput("t", this.t);

			if (a is float af && b is float bf && t is float tf)
			{
				SetOutput("Out", math.lerp(af, bf, tf));
				return;
			}

			var buffA = GPUBufferHandle.Create(a);
			var buffB = GPUBufferHandle.Create(b);
			var buffC = GPUBufferHandle.Create(t);

			var material = MaterialCache.GetMaterial(MaterialCache.Op.Lerp);
			int pass = 0;

			material.SetTexture(HASH_TEXA, buffA);
			material.SetTexture(HASH_TEXB, buffB);
			material.SetTexture(HASH_TEXC, buffC);

			Graphics.Blit(null, buffer, material, pass);

			buffA.ReleaseIfTemp();
			buffB.ReleaseIfTemp();
			buffC.ReleaseIfTemp();

			SetOutput("Out", buffer);
		}
	}
}
