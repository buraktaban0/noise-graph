using System.Collections;
using System.Collections.Generic;
using Procedural.GPU;
using Procedural.Jobs;
using Procedural.Native;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class PowerNode : NoiseGraphNode
	{


		private readonly string shaderName = "Hidden/Procedural/Power";

		[Input] public GPUNoiseBufferHandle a;
		[Input] public float power = 1f;

		[Output] public GPUNoiseBufferHandle result;

		private Material material;

		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUNoiseBufferHandle));
			var power = GetInputValue("x0", this.power);

			if (a.IsCreated == false)
			{
				ClearBuffer();
				return Buffer;
			}

			if (material == null)
				material = new Material(Shader.Find(shaderName));

			material.SetTexture("_TexA", a);
			material.SetFloat("_Power", power);

			Graphics.Blit(null, Buffer, material);

			Buffer.Range = math.pow(a.Range, power);

			isClear = false;

			return Buffer;
		}


	}

}
