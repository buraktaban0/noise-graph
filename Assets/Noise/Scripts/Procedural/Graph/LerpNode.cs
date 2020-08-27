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

		[Input] public GPUBufferHandle a;
		[Input] public GPUBufferHandle b;
		[Input] [Range(0f, 1f)] public float t;

		[Output] public GPUBufferHandle result;


		private Material material;


		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUBufferHandle));
			var b = GetInputValue("b", default(GPUBufferHandle));
			var t = GetInputValue("t", this.t);

			if (a.IsCreated == false && b.IsCreated == false)
			{
				ClearBuffer();
				return Buffer;
			}
			else if (a.IsCreated == false)
			{
				Graphics.Blit(b, Buffer);
				Buffer.Range = b.Range;

				return Buffer;
			}
			else if (b.IsCreated == false)
			{
				Graphics.Blit(a, Buffer);
				Buffer.Range = a.Range;

				return Buffer;
			}


			if (material == null)
			{
				material = new Material(Shader.Find(shaderName));
			}

			material.SetTexture(HASH_TEXA, a);
			material.SetTexture(HASH_TEXB, b);
			material.SetFloat("_Percent", t);

			Graphics.Blit(null, Buffer, material);

			Buffer.Range = math.lerp(a.Range, b.Range, t); // TODO: add a MinMax compute pass

			isClear = false;

			return Buffer;
		}


	}

}
