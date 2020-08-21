using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using System.Linq;

namespace Procedural
{
	[CreateAssetMenu(menuName = "Layered Noise")]
	public class LayeredNoise : BaseNoise
	{

		[HideInInspector]
		public List<NoiseLayer> layers = new List<NoiseLayer>();

		public int seed = 0;

		public bool normalize = true;

		public bool floorToZero = true;

		public bool logTime = false;

		[Range(10, 1000)]
		public int curveSampleCount = 100;

		[Min(0.0001f)]
		public float frequency = 1f;

		[Min(0f)]
		public Vector3 offset = Vector3.zero;

		[HideInInspector]
		public bool isModified;

		public float MinHeight { get; private set; } = -1f;
		public float MaxHeight { get; private set; } = -1f;

		public void AddDefault()
		{
			var layer = new NoiseLayer(this.seed);
			layers.Insert(0, layer);
		}

		public void Prepare()
		{
			Perlin.SetSeed(this.seed);
		}

		public float Sample(float x, float y = 0f, float z = 0f)
		{
			float value = 0f;

			for (int i = 0; i < layers.Count; i++)
			{
				var layer = layers[i];
				var layerValue = layer.Sample(x, y, z);

				switch (layer.blendMode)
				{
					case NoiseLayer.BlendMode.Additive:
						value += layerValue;
						break;
					case NoiseLayer.BlendMode.Multiplicative:
						value *= layerValue;
						break;
				}

			}

			return value;
		}


		public override NativeArray<float> GetHeightmapNative(int width, int height, Allocator nativeHeightmapAllocator)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

			if (logTime)
			{
				sw.Start();
			}

			Perlin.SetSeed(this.seed);

			var nativePerlinBox = Perlin.Native();

			var valuesNative = new NativeArray<float>(width * height, nativeHeightmapAllocator);

			var layers = this.layers.Where(l => l.visible).ToArray();

			var curveDataNative = new NativeArray<float>(layers.SelectMany(l => l.curve.Cache(curveSampleCount)).ToArray(), Allocator.TempJob);
			var mFactorsNative = new NativeArray<int>(layers.Select(l => l.blendMode == NoiseLayer.BlendMode.Multiplicative ? 1 : 0).ToArray(), Allocator.TempJob);
			var aFactorsNative = new NativeArray<int>(layers.Select(l => l.blendMode == NoiseLayer.BlendMode.Additive ? 1 : 0).ToArray(), Allocator.TempJob);
			var amplitudesNative = new NativeArray<float>(layers.Select(l => l.amplitude).ToArray(), Allocator.TempJob);
			var frequenciesNative = new NativeArray<float>(layers.Select(l => math.pow(l.frequency * this.frequency, 2f) * 0.1f).ToArray(), Allocator.TempJob);
			var offsetNative = new NativeArray<float3>(layers.Select(l => (float3)(l.offset + this.offset)).ToArray(), Allocator.TempJob);

			var job = new HeightmapJob
			{
				width = width,
				height = height,
				curveSampleCount = curveSampleCount,
				curveData = curveDataNative,
				mFactors = mFactorsNative,
				aFactors = aFactorsNative,
				amplitudes = amplitudesNative,
				frequencies = frequenciesNative,
				offsets = offsetNative,
				perlin = nativePerlinBox,
				values = valuesNative,
			};

			if (logTime)
			{
				sw.Stop();
				Debug.Log($"LayeredNoise Preparation took {sw.Elapsed.TotalMilliseconds.ToString("0.00")} ms");
				sw.Reset();
				sw.Start();
			}

			var handle = job.Schedule(valuesNative.Length, 8);
			handle.Complete();

			if (logTime)
			{
				sw.Stop();
				Debug.Log($"LayeredNoise Heightmap job took {sw.Elapsed.TotalMilliseconds.ToString("0.00")} ms");
				sw.Reset();
			}

			if (logTime)
			{
				sw.Start();
			}

			var minmaxNative = new NativeArray<float>(2, Allocator.TempJob);

			var minMaxJob = new MinMaxJob
			{
				values = valuesNative,
				minMax = minmaxNative
			};

			minMaxJob.Run();

			float min0 = minmaxNative[0];
			float max0 = minmaxNative[1];

			float min1 = min0;
			float max1 = max0;

			minmaxNative.Dispose();

			if (Mathf.Approximately(min0, max0) == false)
			{

				min1 = floorToZero || normalize ? 0f : min0;
				max1 = normalize ? 1f : floorToZero ? (max0 - min0) : max0;

				var normalizationJob = new NormalizationJob
				{
					min0 = min0,
					max0 = max0,
					min1 = min1,
					max1 = max1,
					values = valuesNative
				};

				var normalizationJobHandle = normalizationJob.Schedule(valuesNative.Length, 8);
				normalizationJobHandle.Complete();

			}

			MinHeight = min1;
			MaxHeight = max1;

			if (logTime)
			{
				sw.Stop();
				Debug.Log($"LayeredNoise Heightmap remapping took {sw.Elapsed.TotalMilliseconds.ToString("0.00")} ms");
				sw.Reset();
			}


			mFactorsNative.Dispose();
			aFactorsNative.Dispose();

			amplitudesNative.Dispose();
			frequenciesNative.Dispose();
			offsetNative.Dispose();

			curveDataNative.Dispose();

			nativePerlinBox.Dispose();

			return valuesNative;

		}

		public float[,] GetHeightmap(int width, int height)
		{

			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

			var valuesNative = GetHeightmapNative(width, height, Allocator.TempJob);

			if (logTime)
			{
				sw.Start();
			}

			float[,] values = new float[width, height];

			for (int i = 0; i < width; i += 1)
			{
				int col = i * height;
				for (int j = 0; j < height; j += 1)
				{
					values[i, j] = valuesNative[col + j];
				}
			}

			if (logTime)
			{
				sw.Stop();
				Debug.Log($"LayeredNoise Heightmap native readback took {sw.Elapsed.TotalMilliseconds.ToString("0.00")} ms");
				sw.Reset();
			}

			valuesNative.Dispose();

			return values;
		}


	}



	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct HeightmapJob : IJobParallelFor
	{

		[ReadOnly]
		public int width;

		[ReadOnly]
		public int height;

		[ReadOnly]
		public int curveSampleCount;

		[ReadOnly]
		public NativeArray<float> curveData;

		[ReadOnly]
		public NativeArray<int> mFactors;

		[ReadOnly]
		public NativeArray<int> aFactors;

		[ReadOnly]
		public NativeArray<float> amplitudes;

		[ReadOnly]
		public NativeArray<float> frequencies;


		[ReadOnly]
		public NativeArray<float3> offsets;

		[ReadOnly]
		public NativePerlinBox perlin;

		[WriteOnly]
		public NativeArray<float> values;

		public void Execute(int index)
		{
			int x = index / height;
			int y = index % height;

			int layerCount = amplitudes.Length;

			float value = 0f;

			for (int i = 0; i < layerCount; i += 1)
			{
				int mFactor = mFactors[i];
				int aFactor = aFactors[i];

				float freq = frequencies[i];
				float amp = amplitudes[i];
				float3 offset = offsets[i];
				float perlinValue = perlin.Sample(x * freq + offset.x, y * freq + offset.y, 0f + offset.z);
				float layerValue = EvaluateCurve(i, perlinValue) * perlinValue * amp;

				value = (value + layerValue) * aFactor + (value * layerValue) * mFactor;
			}

			values[index] = value;

		}


		private float EvaluateCurve(int layer, float t)
		{
			float sampleStep = 1f / (curveSampleCount - 1);
			t = math.clamp(t, 0f, 0.9999f);
			float tf = (t / sampleStep);
			int _i0 = (int)tf;

			int i0 = curveSampleCount * layer + _i0;
			int i1 = i0 + 1;

			float c0 = curveData[i0];
			float c1 = curveData[i1];

			float _t = tf - _i0;

			float c = math.lerp(c0, c1, _t);

			return c;
		}

	}



	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct NormalizationJob : IJobParallelFor
	{

		[ReadOnly]
		public float min0;
		[ReadOnly]
		public float max0;

		[ReadOnly]
		public float min1;
		[ReadOnly]
		public float max1;

		public NativeArray<float> values;

		public void Execute(int index)
		{
			values[index] = math.remap(min0, max0, min1, max1, values[index]);
		}

	}

	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct MinMaxJob : IJob
	{
		[ReadOnly]
		public NativeArray<float> values;

		public NativeArray<float> minMax;

		public void Execute()
		{

			float min = float.MaxValue;
			float max = float.MinValue;

			for (int i = 0; i < values.Length; i++)
			{
				float value = values[i];
				min = math.min(min, value);
				max = math.max(max, value);
			}

			minMax[0] = min;
			minMax[1] = max;

		}

	}

}


