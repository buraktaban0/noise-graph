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
			}

			material.SetTexture("_TexA", a);
			material.SetTexture("_TexB", b);

			Graphics.Blit(null, buffer, material);

			buffer.Range = a.Range - new float2(b.Range.y, b.Range.x);

			isClear = false;

			return buffer;
		}


	}

}