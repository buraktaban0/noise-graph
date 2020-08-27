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

	public class CurveTransformNode : NoiseGraphNode
	{

		private readonly string shaderName = "Hidden/Procedural/CurveTransform";

		public CurveTransformParams parameters;

		[Input] public GPUBufferHandle a;
		[Output] public GPUBufferHandle result;

		private float[] curveData;

		private Material material;

		protected override void Init()
		{
			base.Init();
		}



		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUBufferHandle));

			if (material == null)
			{
				material = new Material(Shader.Find(shaderName));
			}

			if (isDirty || curveData == null || curveData.Length != parameters.sampleCount)
			{
				curveData = parameters.curve.Cache(parameters.sampleCount);
				material.SetFloatArray("_CurveData", curveData);
				material.SetInt("_SampleCount", parameters.sampleCount);
			}

			if (a.IsCreated == false)
			{
				ClearBuffer();
				return Buffer;
			}

			material.SetTexture("_TexA", a);

			Graphics.Blit(null, Buffer, material);

			isClear = false;

			return Buffer;
		}


	}

}