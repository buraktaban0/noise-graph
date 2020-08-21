using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Procedural.Jobs
{

	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct PerlinJob : IJobParallelFor
	{

		[ReadOnly]
		public int width;

		[ReadOnly]
		public int height;

		[ReadOnly]
		public float amplitude;

		[ReadOnly]
		public float frequency;

		[ReadOnly]
		public float2 offset;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{

			result[index] = math.mad(noise.cnoise(new float2(index / height, index % height) * frequency + offset), 0.4999f, 0.5f) * amplitude;

		}



	}

	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct PerlinJobTest : IJobParallelFor
	{

		[ReadOnly]
		public int width;

		[ReadOnly]
		public int height;

		[ReadOnly]
		public float amplitude;

		[ReadOnly]
		public float frequency;

		[ReadOnly]
		public float2 offset;

		[ReadOnly]
		public NativeArray<float2> positions;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{

			//result[index] = math.mad(noise.cnoise(new float2(index / height, index % height) * frequency + offset), 0.4999f, 0.5f) * amplitude;
			float2 f = positions[index];
			float n = noise.cnoise(f);
			result[index] = n;

		}



	}



}
