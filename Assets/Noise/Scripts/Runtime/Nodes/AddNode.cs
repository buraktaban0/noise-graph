using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using Noise.Runtime.Util;
using Procedural.GPU;
using UnityEngine;

namespace Noise.Runtime.Nodes
{
	public class AddNode : NoiseGraphNode
	{

		private static readonly string shaderName = "Procedural/Add";


		[Input(typeof(GPUNoiseBufferHandle))]
		public float A;

		[Input(typeof(GPUNoiseBufferHandle))]
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
				SetOutput("Out", af + bf);
				return;
			}

			var material = MaterialCache.GetMaterial(shaderName);
			int pass = 1;

			if (a is float af2)
			{
				material.SetFloat(HASH_VALUE, af2);
				material.SetTexture(HASH_TEXA, (GPUNoiseBufferHandle)b);
			}
			else if (b is float bf2)
			{
				material.SetTexture(HASH_TEXA, (GPUNoiseBufferHandle)a);
				material.SetFloat(HASH_VALUE, bf2);
			}
			else
			{
				material.SetTexture(HASH_TEXA, (GPUNoiseBufferHandle)a);
				material.SetTexture(HASH_TEXB, (GPUNoiseBufferHandle)b);
				pass = 0;
			}

			Graphics.Blit(null, buffer, material, pass);

			SetOutput("Out", buffer);

		}

	}
}
