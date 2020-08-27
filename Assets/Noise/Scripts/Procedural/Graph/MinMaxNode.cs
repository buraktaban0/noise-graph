using System.Collections;
using System.Collections.Generic;
using Procedural.GPU;
using Procedural.Native;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class MinMaxNode : NoiseGraphNode
	{
		private readonly string shaderName = "Hidden/Procedural/MinMax";

		[Input] public GPUBufferHandle a;

		[Output] public float2 result;

		private ComputeShader cs;

		private int kernelIndex;

		private Material material;

		public override bool ShouldCreateBuffer() => false;

		public struct test
		{
			public uint x;
			public uint y;
			public int lum;
		}

		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUBufferHandle));

			if (a.IsCreated == false)
			{
				return 0f;
			}

			/*
			if (material == null)
			{
				material = new Material(Shader.Find(shaderName));
			}

			var reducedRT = Reduce(a);
			var rt = RenderTexture.active;
			Graphics.SetRenderTarget(reducedRT);
			Texture2D tex = new Texture2D(1, 1, TextureFormat.RGFloat, false);
			tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0, false);
			tex.Apply(false );
			var nat = tex.GetRawTextureData<float2>();

			float2 val = nat[0];
			Debug.Log(val);

			Graphics.SetRenderTarget(rt);

			return val;
			*/

			if (cs == null)
			{
				cs = Resources.Load<ComputeShader>("CS_MinMax");
				kernelIndex = cs.FindKernel("FindBrights");
			}

			var sw = new System.Diagnostics.Stopwatch();

			int w = 32;
			var buffer = new ComputeBuffer(w * w, 4, ComputeBufferType.Append);
			//buffer.SetData(new int2[] { -2 });

			cs.SetTexture(kernelIndex, "InputTexture", a);
			//cs.SetInt("InputTextureWidth", 1024);
			cs.SetBuffer(kernelIndex, "_brightPoints", buffer);

			int[] arr = new int[w * w];


			sw.Start();

			cs.Dispatch(kernelIndex, w, w, 1);
			buffer.GetData(arr);
			

			sw.Stop();

			Debug.Log(sw.Elapsed.TotalMilliseconds);

			int groupMax = 0;
			for (int group = 1; group < w * w; group++)
			{
				if (arr[group] > arr[groupMax])
				{
					groupMax = group;
				}
			}

			float lum = arr[groupMax] / 1024f;

			Debug.Log(lum);

			buffer.Release();

			return new float2(0f, 1f);

			/*int2 minMaxInt = arr[0];
			float2 minMax = (float2)minMaxInt / 512f;

			//buffer.Dispose();

			return minMax;*/
		}

		private RenderTexture Reduce(RenderTexture diff)
		{
			int reductions = (int)System.Math.Log(diff.width, 2);
			var rtList = new List<RenderTexture>();
			int smallRes = diff.width / 2;
			var bigRT = diff;

			var smallRT = RenderTexture.GetTemporary(smallRes, smallRes, 0, RenderTextureFormat.RGFloat);

			rtList.Add(smallRT);
			Graphics.Blit(bigRT, smallRT, material, 0);
			smallRes >>= 1;
			bigRT = smallRT;
			for (int i = 1; i < reductions; i++)
			{
				smallRT = RenderTexture.GetTemporary(smallRes, smallRes, 0, diff.format);
				rtList.Add(smallRT);
				Graphics.Blit(bigRT, smallRT, material, 1);
				bigRT = smallRT;
			}
			for (int i = 0; i < reductions - 1; i++)
			{
				RenderTexture.ReleaseTemporary(rtList[i]);
			}
			return rtList[reductions - 1];
		}

	}

}
