using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Procedural.Graph
{
	[System.Serializable]
	public class CurveTransformParams
	{
		public bool useCurve = false;

		public AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
		[Range(128, 1024)] public int sampleCount = 300;

		public enum Mode : int
		{
			EvaluateOnly = 0,
			EvaluateAndMultiply = 1
		}

		private readonly string[] KEYWORDS = {
			"EVALUATE_ONLY",
			"EVALUATE_AND_MULTIPLY"
		};

		public Mode mode;

		internal void SetShaderKeywords(Material material)
		{
			for (int i = 0; i < KEYWORDS.Length; i++)
			{
				if (i != (int)mode)
				{
					material.DisableKeyword(KEYWORDS[i]);
				}
			}
			material.EnableKeyword(KEYWORDS[(int)mode]);
		}
	}
}
