﻿using System;
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
	public struct RemapJob : IJobParallelFor
	{

		[ReadOnly]
		public float4 map;

		[ReadOnly]
		public NativeArray<float> a;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{
			result[index] = math.remap(map.x, map.y, map.z, map.w, a[index]);
		}
	}
}
