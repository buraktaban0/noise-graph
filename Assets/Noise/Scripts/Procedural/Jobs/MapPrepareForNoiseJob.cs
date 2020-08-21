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
	public struct MapPrepareForNoiseJob : IJobParallelFor
	{
		[ReadOnly]
		public uint height;

		[WriteOnly]
		public NativeArray<float4> result;

		public void Execute(int index)
		{
			result[index] = new float4(index / height, index % height, 0f, 0f);
		}

	}
}
