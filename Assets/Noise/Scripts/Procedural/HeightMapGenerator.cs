using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Procedural
{

	public class HeightMapGenerator
	{

		[System.Serializable]
		public class Settings
		{

			public int seed = 0;

			[Range(3, 8192)]
			public int width = 4;

			[Range(3, 8192)]
			public int height = 4;

			[Range(1, 5)]
			public int octaves = 2;

			[Range(0.01f, 10f)]
			public float frequency = 1f;

			[Range(1f, 4f)]
			public float lacunarity = 2f;

			[Range(0.1f, 2f)]
			public float persistance = 0.5f;

			public Settings(int seed, int width, int height, int octaves, float frequency, float lacunarity, float persistance)
			{
				this.seed = seed;
				this.width = width;
				this.height = height;
				this.octaves = octaves;
				this.frequency = frequency;
				this.lacunarity = lacunarity;
				this.persistance = persistance;
			}

		}

		public Settings settings { get; set; }

		public HeightMapGenerator(Settings settings)
		{
			this.settings = settings;
		}


		public float[,] Generate()
		{
			Perlin.SetSeed(settings.seed);

			float[,] values = new float[settings.width, settings.height];

			float min = float.MaxValue;
			float max = float.MinValue;

			for (int i = 0; i < settings.width; i += 1)
			{
				float x = (float)i * settings.frequency * 0.1f;
				for (int j = 0; j < settings.height; j += 1)
				{
					float y = (float)j * settings.frequency * 0.1f;

					float value = Perlin.OctavePerlin(settings.octaves, settings.persistance, settings.lacunarity, x, y);

					min = Mathf.Min(value, min);
					max = Mathf.Max(value, max);

					values[i, j] = value;
				}
			}


			for (int i = 0; i < settings.width; i += 1)
			{
				for (int j = 0; j < settings.height; j += 1)
				{
					//values[i, j] = Mathf.SmoothStep(min, max, values[i, j]);
				}
			}

			return values;
		}


	}

}
