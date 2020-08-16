using System.Collections;
using System.Collections.Generic;
using Procedural.GPU;
using Procedural.Jobs;
using Procedural.Native;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class LerpNode : NoiseGraphNode
	{

		private readonly string shaderName = "Hidden/Procedural/Lerp";

		[Input] public GPUNoiseBufferHandle a;
		[Input] public GPUNoiseBufferHandle b;
		[Input] [Range(0f, 1f)] public float t;

		[Output] public GPUNoiseBufferHandle result;


		private Material material;


		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUNoiseBufferHandle));
			var b = GetInputValue("b", default(GPUNoiseBufferHandle));
			var t = GetInputValue("t", this.t);

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
			}

			material.SetTexture(hashTexA, a);
			material.SetTexture(hashTexB, b);
			material.SetFloat("_Percent", t);

			Graphics.Blit(null, buffer, material);

			buffer.Range = math.lerp(a.Range, b.Range, t); // TODO: add a MinMax compute pass

			isClear = false;

			return buffer;
		}


	}

}
