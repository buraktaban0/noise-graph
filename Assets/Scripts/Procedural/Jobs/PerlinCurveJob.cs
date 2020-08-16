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
	public struct PerlinCurveJob : IJobParallelFor
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
		public float curveSampleStep;

		[ReadOnly]
		public NativeArray<float> curveData;

		[WriteOnly]
		public NativeArray<float> result;

		public void Execute(int index)
		{

			float xn = noise.cnoise(new float2(index / height, index % height) * frequency + offset);
			float xf = math.mad(xn, 0.49999f, 0.5f) / curveSampleStep;

			int xi0 = (int)xf;
			result[index] = math.lerp(curveData[xi0], curveData[xi0 + 1], math.frac(xf)) * amplitude;

		}



	}



}
