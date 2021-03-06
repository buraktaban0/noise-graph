﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Procedural.Jobs
{
	public struct InitializeWithZeroJob<T> : IJobParallelFor where T : struct
	{
		[WriteOnly]
		public NativeArray<float> a;

		public void SetTarget(NativeArray<T> target, int expectedSize)
		{
			a = target.Reinterpret<float>();
		}

		public void Execute(int index)
		{
			a[index] = 0f;
		}
	}
}
