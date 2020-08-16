using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Procedural
{

	public static class Utility
	{

		public static float[] Cache(this AnimationCurve curve, int sampleCount)
		{
			float[] values = new float[sampleCount];

			for (int i = 0; i < sampleCount; i++)
			{
				float t = (float)i / (sampleCount - 1);
				values[i] = curve.Evaluate(t);
			}

			return values;
		}

		public static NativeArray<float> CacheNative(this AnimationCurve curve, int sampleCount, Allocator allocator)
		{
			NativeArray<float> values = new NativeArray<float>(sampleCount, allocator);

			for (int i = 0; i < sampleCount; i++)
			{
				float t = (float)i / (sampleCount - 1);
				values[i] = curve.Evaluate(t);
			}

			return values;
		}

	}

}
