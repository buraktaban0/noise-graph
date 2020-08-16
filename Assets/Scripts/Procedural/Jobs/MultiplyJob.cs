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
	public struct MultiplyJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<float> a;

		[ReadOnly]
		public NativeArray<float> b;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{
			result[index] = math.mul(a[index], b[index]);
		}
	}

	/*
	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct MultiplyJobTest : IJob
	{
		[ReadOnly]
		public NativeArray<float> a;

		[ReadOnly]
		public NativeArray<float> b;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute()
		{
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = math.mul(a[i], b[i]);
			}
		}
	}

	[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
	public struct MultiplyJobTest2 : IJob
	{
		[ReadOnly]
		public NativeArray<float> a;

		[ReadOnly]
		public NativeArray<float> b;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute()
		{
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = a[i] * b[i];
			}
		}
	}
	*/

}
