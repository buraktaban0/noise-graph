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
	public struct CurveTransformJob : IJobParallelFor
	{

		[ReadOnly]
		public float sampleStep;

		[ReadOnly]
		public NativeArray<float> curveData;

		[ReadOnly]
		public NativeArray<float> a;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{
			float xf = math.clamp(a[index], 0f, 0.9999f) / sampleStep;
			int xi0 = (int)xf;
			result[index] = math.lerp(curveData[xi0], curveData[xi0 + 1], math.frac(xf));
		}

	}
}
