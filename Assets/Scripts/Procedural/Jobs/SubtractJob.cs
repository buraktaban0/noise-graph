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
	public struct SubtractJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<float> a;

		[ReadOnly]
		public NativeArray<float> b;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{
			result[index] = a[index] - b[index];
		}
	}
}
