using System.Collections;
using System.Collections.Generic;
using Procedural.GPU;
using Procedural.Native;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class MultiplyNode : NoiseGraphNode
	{
		private readonly string shaderName = "Hidden/Procedural/Multiply";

		[Input] public GPUNoiseBufferHandle a;
		[Input] public GPUNoiseBufferHandle b;

		[Output] public GPUNoiseBufferHandle result;


		private Material material;

		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUNoiseBufferHandle));
			var b = GetInputValue("b", default(GPUNoiseBufferHandle));

			if (a.IsCreated == false && b.IsCreated == false)
			{
				ClearBuffer();
				return buffer;
			}
			else if (a.IsCreated == false)
			{
				Graphics.Blit(b, buffer);
				buffer.Range = b.Range;

				return buffer;
			}
			else if (b.IsCreated == false)
			{
				Graphics.Blit(a, buffer);
				buffer.Range = a.Range;

				return buffer;
			}


			if (material == null)
			{
				material = new Material(Shader.Find(shaderName));
				material.EnableKeyword("TEX_MODE");
			}

			material.SetTexture("_TexA", a);
			material.SetTexture("_TexB", b);

			Graphics.Blit(null, buffer, material);

			float[] edges = new float[]{
				a.Range.x * b.Range.x,
				a.Range.x * b.Range.y,
				a.Range.y * b.Range.y
			};

			buffer.Range = new float2(Mathf.Min(edges), Mathf.Max(edges));

			return buffer;
		}


	}

}
