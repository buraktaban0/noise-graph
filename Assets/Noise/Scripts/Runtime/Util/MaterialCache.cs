using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Noise.Runtime.Util
{

	public static class MaterialCache
	{

		private static readonly string[] shaders =
			{
			"Procedural/Add",
			"Procedural/Multiply",
			"Procedural/Clamp",
			"Procedural/Lerp",
		};

		private static Dictionary<string, Material> materials;

		static MaterialCache()
		{
			materials = shaders.Select(s => new Material(Shader.Find(s))).ToDictionary(m => m.shader.name);
		}

		public static Material GetMaterial(string shaderName)
		{
			return materials[shaderName];
		}

	}

}
