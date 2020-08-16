using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Procedural
{

	[CreateAssetMenu(menuName = "Noise/Tree Noise", fileName = "TreeNoise")]
	public class TreeNoise : BaseNoise
	{

		[HideInInspector] public List<LayeredNoise> branches = new List<LayeredNoise>();

		public override NativeArray<float> GetHeightmapNative(int width, int height, Allocator nativeAllocator)
		{

			var branches = this.branches.Where(b => b != null).ToArray();

			int branchCount = branches.Length;

			int flatArrayLength = width * height * branchCount;

			var heightmaps = branches.Select(b => b.GetHeightmapNative(width, height, Allocator.Persistent)).ToArray();

			var flatHeightmap = new NativeArray<float>(flatArrayLength, Allocator.TempJob);

			var heightmap = new NativeArray<float>(width * height, nativeAllocator);

			for (int i = 0; i < branchCount; i++)
			{
				NativeArray<float>.Copy(heightmaps[i], 0, flatHeightmap, i * width * height, width * height);
			}


			var sumJob = new Flat2DSumJob
			{
				depthCount = branchCount,
				depthLength = width * height,
				flat = flatHeightmap,
				sum = heightmap
			};

			var sumJobHandle = sumJob.Schedule(width * height, 8);

			sumJobHandle.Complete();

			return heightmap;

		}

	}

}


[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
public struct Flat2DSumJob : IJobParallelFor
{

	[ReadOnly]
	public NativeArray<float> flat;

	[ReadOnly]
	public int depthCount;

	[ReadOnly]
	public int depthLength;

	[WriteOnly]
	public NativeArray<float> sum;


	public void Execute(int index)
	{

		float value = 0f;
		for (int i = 0; i < depthCount; i++)
		{
			value += flat[depthLength * i + index];
		}

		sum[index] = value;

	}

}
