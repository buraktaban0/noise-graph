using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaughtyAttributes;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procedural.Jobs.Test
{
	public class TestJob : MonoBehaviour
	{

		public RenderTexture rt1, rt2, rt3;

		public Material material;

		[Button]
		public void DoTest()
		{

			for (int i = 0; i < 10000; i++)
			{
				typeof(float3).GetMethod("op_Addition", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
					.Invoke(null, new object[] { 2f, i });
			}

			return;

			int size = 1024;

			if (rt1 == null || rt1.width != size)
			{
				rt1 = new RenderTexture(size, size, 0, RenderTextureFormat.RFloat);
				rt1.Create();
			}

			if (rt2 == null || rt2.width != size)
			{
				rt2 = new RenderTexture(size, size, 0, RenderTextureFormat.RFloat);
				rt2.Create();
			}

			if (rt3 == null || rt3.width != size)
			{
				rt3 = new RenderTexture(size, size, 0, RenderTextureFormat.RFloat);
				rt3.Create();
			}

			material.SetFloat("_Frequency", 2f);
			material.SetFloat("_Amplitude", 1f);
			material.SetVector("_Offset", new Vector3(0f, 0f, 0f));

			var addMat = new Material(Shader.Find("Hidden/Procedural/Add"));
			addMat.SetTexture("_TexA", rt1);
			addMat.SetTexture("_TexB", rt2);

			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();


			Graphics.Blit(null, rt1, material);
			Graphics.Blit(null, rt2, material);

			Graphics.Blit(null, rt3, addMat);

			sw.Stop();

			Debug.Log(sw.Elapsed.TotalMilliseconds);

			/*
			var a = new NativeArray<float>(1024 * 1024 * 256, Allocator.Persistent);
			var b = new NativeArray<float>(1024 * 1024 * 256, Allocator.Persistent);
			var r = new NativeArray<float>(1024 * 1024 * 256, Allocator.Persistent);
			var v = new NativeArray<float>(1024 * 1024, Allocator.Persistent);
			*/

		}


	}


	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct AddA : IJobParallelFor
	{
		public NativeArray<float> a;


		[Unity.Collections.ReadOnly]
		public NativeArray<float> b;

		public void Execute(int index)
		{
			a[index] = a[index] + b[index];
		}

	}

	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct AddB : IJob
	{

		public NativeArray<float> a;

		[Unity.Collections.ReadOnly]
		public NativeArray<float> b;

		public void Execute()
		{
			int len = a.Length;
			for (int i = 0; i < len; i++)
			{
				a[i] = a[i] + b[i];
			}
		}

	}



	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct Pack : IJobParallelFor
	{
		[WriteOnly]
		public NativeArray<float4> a;

		[Unity.Collections.ReadOnly]
		public NativeArray<float> b;

		public void Execute(int index)
		{
			float4 p = new float4(
				b[index * 4],
				b[index * 4 + 1],
				b[index * 4 + 2],
				b[index * 4 + 3]
				);

			a[index] = p;
		}

	}


}
