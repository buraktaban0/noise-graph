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
	public class ClampNode : NoiseGraphNode
	{

		private readonly string clampShaderName = "Hidden/Procedural/Clamp";

		[Input] public GPUNoiseBufferHandle a;
		[Input] public float x0 = 0f;
		[Input] public float x1 = 1f;

		[Output] public GPUNoiseBufferHandle result;

		private Material clampMaterial;

		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var x0 = GetInputValue("x0", this.x0);
			var x1 = GetInputValue("x1", this.x1);
			var a = GetInputValue("a", default(GPUNoiseBufferHandle));

			if (a.IsCreated == false)
			{
				ClearBuffer();
				return Buffer;
			}

			if (clampMaterial == null)
				clampMaterial = new Material(Shader.Find(clampShaderName));

			clampMaterial.SetTexture("_TexA", a);
			clampMaterial.SetVector("_Range", new Vector2(x0, x1));

			Graphics.Blit(null, Buffer, clampMaterial);

			Buffer.Range = math.clamp(a.Range, x0, x1);

			isClear = false;

			return Buffer;
		}


	}

}