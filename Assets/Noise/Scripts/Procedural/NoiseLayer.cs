using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Procedural
{

	[System.Serializable]
	public class NoiseLayer
	{

		public enum BlendMode : int
		{
			Additive = 0,
			Multiplicative = 1
		}

		public AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		public BlendMode blendMode = BlendMode.Additive;

		public float amplitude = 1f;

		[Min(0.0001f)]
		public float frequency = 0.1f;

		[Min(0f)]
		public Vector3 offset = Vector3.zero;

		public bool visible = true;

		private int seed = 0;

		public NoiseLayer(int seed)
		{
			this.seed = seed;
		}

		public float Sample(float x, float y = 0f, float z = 0f)
		{

			var perlin = Perlin.perlin(x * frequency + offset.x, y * frequency + offset.y, z * frequency + offset.z);
			//return perlin * amplitude;
			return curve.Evaluate(perlin) * perlin * amplitude;
		}

	}




}
