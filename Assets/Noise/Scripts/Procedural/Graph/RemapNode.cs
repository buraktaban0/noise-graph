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

	public class RemapNode : NoiseGraphNode
	{

		private readonly string shaderName = "Hidden/Procedural/Remap";

		[Input] public GPUNoiseBufferHandle a;
		[Input] public float2 from = new float2(0f, 1f);
		[Input] public float2 to = new float2(0f, 2f);

		[Output] public GPUNoiseBufferHandle result;

		private Material material;

		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUNoiseBufferHandle));
			var from = GetInputValue("from", this.from);
			var to = GetInputValue("to", this.to);

			if (a.IsCreated == false)
			{
				ClearBuffer();
				return Buffer;
			}

			if (material == null)
				material = new Material(Shader.Find(shaderName));

			material.SetTexture("_TexA", a);
			material.SetVector("_Map", new Vector4(from.x, from.y, to.x, to.y));

			Graphics.Blit(null, Buffer, material);

			Buffer.Range = math.remap(from.x, from.y, to.x, to.y, a.Range);

			isClear = false;

			return Buffer;
		}


	}

}