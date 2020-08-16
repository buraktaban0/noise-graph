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
	public struct MapApplyFrequencyAndOffsetJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<float4> a;

		[ReadOnly]
		public float4 frequency;

		[ReadOnly]
		public float4 offset;

		[WriteOnly]
		public NativeArray<float4> result;

		public void Execute(int index)
		{
			result[index] = a[index] * frequency + offset;
		}
	}

	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct MapApplyFrequencyAndOffsetJobTest : IJob
	{
		[ReadOnly]
		public NativeArray<float4> a;

		[ReadOnly]
		public float4 frequency;

		[ReadOnly]
		public float4 offset;

		[WriteOnly]
		public NativeArray<float4> result;

		public void Execute()
		{
			for (int index = 0; index < result.Length; index++)
			{
				result[index] = a[index] * frequency + offset;
			}
		}
	}

}
