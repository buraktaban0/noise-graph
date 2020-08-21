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

	public class PerlinNode : NoiseGraphNode
	{
		private readonly string perlinShader = "Hidden/Procedural/Perlin";
		private readonly string multiplyShader = "Hidden/Procedural/Multiply";
		private readonly string curveTransformShader = "Hidden/Procedural/CurveTransform";


		public CurveTransformParams curveParameters;

		[Min(0.5f)]
		[Input] public float frequency = 10f;
		[Input] public float amplitude = 1f;
		[Input] public float3 offset = 0f;

		[Output] public GPUNoiseBufferHandle result;

		private float[] curveData;

		private Material perlinMaterial;

		private Material multiplyMaterial;

		private Material curveMaterial;

		private CurveTransformParams.Mode previousMode;

		private RenderTexture tmpRt;

		protected override void Init()
		{
			base.Init();

			var size = GetBufferSize();
			tmpRt = new RenderTexture(size, size, 0, RenderTextureFormat.RFloat);
			tmpRt.Create();

			isDirty = true;
		}


		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			if (!isDirty)
			{
				return Buffer;
			}

			isDirty = false;

			float freq = GetInputValue<float>("frequency", this.frequency);
			float amp = GetInputValue<float>("amplitude", this.amplitude);
			float3 offset = GetInputValue<float3>("offset", this.offset);

			if (curveMaterial == null)
			{
				curveMaterial = new Material(Shader.Find(curveTransformShader));
			}

			if (isDirty || curveData == null || curveData.Length != curveParameters.sampleCount || previousMode != curveParameters.mode)
			{
				curveData = curveParameters.curve.Cache(curveParameters.sampleCount);
				curveMaterial.SetFloatArray("_CurveData", curveData);
				curveMaterial.SetInt("_SampleCount", curveParameters.sampleCount);
				curveMaterial.SetTexture("_TexA", Buffer);
				previousMode = curveParameters.mode;
				var keyword = previousMode == CurveTransformParams.Mode.EvaluateOnly ? "EVALUATE_ONLY" : "EVALUATE_AND_MULTIPLY";

				curveMaterial.EnableKeyword(keyword);
			}


			if (perlinMaterial == null)
				perlinMaterial = new Material(Shader.Find(perlinShader));


			perlinMaterial.SetFloat("_Frequency", freq);
			perlinMaterial.SetVector("_Offset", (Vector3)offset);

			if (multiplyMaterial == null)
			{
				multiplyMaterial = new Material(Shader.Find(multiplyShader));
				multiplyMaterial.SetTexture("_TexA", Buffer);
				multiplyMaterial.EnableKeyword("FLOAT_MODE");
			}

			multiplyMaterial.SetFloat("_Factor", amp);


			if (Mathf.Approximately(0f, amp))
			{
				ClearBuffer();
				return Buffer;
			}

			Graphics.Blit(null, Buffer, perlinMaterial);

			if (curveParameters.useCurve)
			{

				curveParameters.SetShaderKeywords(curveMaterial);
				curveData = curveParameters.curve.Cache(curveParameters.sampleCount);
				curveMaterial.SetFloatArray("_CurveData", curveData);
				curveMaterial.SetInt("_SampleCount", curveParameters.sampleCount);
				curveMaterial.SetTexture(HASH_TEXA, Buffer);
				Graphics.Blit(null, tmpRt, curveMaterial);
				Graphics.Blit(tmpRt, Buffer);
			}

			if (!Mathf.Approximately(1f, amp))
			{
				multiplyMaterial.SetTexture(HASH_TEXA, Buffer);
				Graphics.Blit(null, tmpRt, multiplyMaterial);
				Graphics.Blit(tmpRt, Buffer);
			}


			Buffer.Range = amp > 0f ? new float2(0f, amp) : new float2(amp, 0f);

			isClear = false;

			return Buffer;

		}


	}

}
