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
			Lerp = 5
		}


		private static readonly (string, Op)[] Shaders =
		{
			("Procedural/Add", Op.Add),
			("Procedural/Subtract", Op.Subtract),
			("Procedural/Multiply", Op.Multiply),
			("Procedural/Clamp", Op.Clamp),
			("Procedural/Lerp", Op.Lerp),
		};

		private static readonly Dictionary<Op, Material> Materials;

		static MaterialCache()
		{
			Materials = new Dictionary<Op, Material>();
			foreach (var (item1, item2) in Shaders)
			{
				var mat = new Material(Shader.Find(item1));
				Materials.Add(item2, mat);
			}
		}

		public static Material GetMaterial(Op operation)
		{
			return Materials[operation];
		}
	}
}
