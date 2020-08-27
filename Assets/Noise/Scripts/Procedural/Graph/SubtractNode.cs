using System.Collections;
using System.Collections.Generic;
using Procedural.GPU;
using Procedural.Native;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class SubtractNode : NoiseGraphNode
	{


		private readonly string shaderName = "Hidden/Procedural/Subtract";

		[Input] public GPUBufferHandle a;
		[Input] public GPUBufferHandle b;

		[Output] public GPUBufferHandle result;


		private Material material;


		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUBufferHandle));
			var b = GetInputValue("b", default(GPUBufferHandle));

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

			material.SetTexture("_TexA", a);
			material.SetTexture("_TexB", b);

			Graphics.Blit(null, Buffer, material);

			Buffer.Range = a.Range - new float2(b.Range.y, b.Range.x);

			isClear = false;

			return Buffer;
		}


	}

}