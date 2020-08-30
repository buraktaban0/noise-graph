using System.Collections.Generic;
using UnityEngine;

namespace Noise.Runtime.Util
{
	public static class MaterialCache
	{
		public enum Op : int
		{
			Add = 1,
			Subtract = 2,
			Multiply = 3,
			Clamp = 4,
			Lerp = 5,
			SingleToMultiChannel = 6,
			Clear = 7,
			Power = 8,
		}


		private static readonly (string, Op)[] Shaders =
		{
			("Clear", Op.Clear),
			("SingleToMulti", Op.SingleToMultiChannel),
			("Add", Op.Add),
			("Subtract", Op.Subtract),
			("Multiply", Op.Multiply),
			("Clamp", Op.Clamp),
			("Lerp", Op.Lerp),
			("Power", Op.Power),
		};

		private static readonly Dictionary<Op, Material> Materials;

		static MaterialCache()
		{
			Materials = new Dictionary<Op, Material>();
			foreach (var (item1, item2) in Shaders)
			{
				var shaderName = $"Hidden/Procedural/{item1}";
				var mat = new Material(Shader.Find(shaderName));
				Materials.Add(item2, mat);
			}
		}

		public static Material GetMaterial(Op operation)
		{
			return Materials[operation];
		}
	}
}
