using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class CalculateVolume : MonoBehaviour
{

	public int sampleCount = 16;


	void Start()
	{
		var mesh = this.GetComponent<MeshFilter>().sharedMesh;

		var vol = GetVolume(mesh, this.sampleCount);

		Debug.Log($"Volume of {mesh.name} : {vol.ToString("0.000")} m^3");

	}


	public float GetVolume(Mesh mesh, int sampleCount)
	{
		mesh.RecalculateBounds();

		int numSteps = sampleCount * sampleCount * sampleCount;

		var verticesNative = new NativeArray<float3>(mesh.vertices.Select(v => new float3(v.x, v.y, v.z)).ToArray(), Allocator.Persistent);
		var indicesNative = new NativeArray<int>(mesh.triangles, Allocator.Persistent);

		var samplesNative = new NativeArray<int>(numSteps, Allocator.Persistent);

		var resultNative = new NativeArray<int>(4, Allocator.Persistent);

		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();

		var bounds = mesh.bounds;


		float3 min = bounds.min;
		float3 step = bounds.size / sampleCount;
		min += step * 0.5f;
		float3 stepX = new float3(step.x, 0f, 0f);
		float3 stepY = new float3(0f, step.y, 0f);
		float3 stepZ = new float3(0f, 0f, step.z);


		var samplePos = new NativeArray<float3>(numSteps, Allocator.Persistent);

		var volumeJob = new VolumeCalculationJob
		{
			vertices = verticesNative,
			indices = indicesNative,
			sampleCount = sampleCount,
			min = min,
			stepX = stepX,
			stepY = stepY,
			stepZ = stepZ,
			samples = samplesNative,
			samplePos = samplePos
		};

		var volumeHandle = volumeJob.Schedule(numSteps, 8);

		var sumJob = new SumJob
		{
			values = samplesNative,
			result = resultNative
		};

		var sumHandle = sumJob.Schedule(volumeHandle);

		volumeHandle.Complete();
		sumHandle.Complete();

		float volume = resultNative[0] * step.x * step.y * step.z;

		sw.Stop();

		Debug.Log($"Volume calculated in {sw.Elapsed.TotalMilliseconds} ms");

		samplePos.Dispose();

		verticesNative.Dispose();
		indicesNative.Dispose();
		samplesNative.Dispose();
		resultNative.Dispose();

		return volume;
	}


}


[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
public struct VolumeCalculationJob : IJobParallelFor
{

	[ReadOnly]
	public NativeArray<float3> vertices;

	[ReadOnly]
	public NativeArray<int> indices;

	[ReadOnly]
	public float3 min;

	[ReadOnly]
	public float3 stepX;
	[ReadOnly]
	public float3 stepY;
	[ReadOnly]
	public float3 stepZ;

	[ReadOnly]
	public int sampleCount;

	[WriteOnly]
	public NativeArray<int> samples;

	[WriteOnly]
	public NativeArray<float3> samplePos;

	public void Execute(int index)
	{
		int x = index % sampleCount;
		int y = (index % (sampleCount * sampleCount)) / sampleCount;
		int z = index / (sampleCount * sampleCount);

		int triCount = indices.Length / 3;

		bool isIn = true;

		float3 p = min + stepX * x + stepY * y + stepZ * z;

		samplePos[index] = p;

		for (int t = 0; t < triCount; t++)
		{
			float3 v1 = vertices[indices[t * 3]];
			float3 v2 = vertices[indices[t * 3 + 1]];
			float3 v3 = vertices[indices[t * 3 + 2]];

			Plane plane = new Plane(v1, v2, v3);

			if (plane.GetSide(p))
			{
				isIn = false;
				break;
			}

		}

		samples[index] = isIn ? 1 : 0;



	}

}


[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
public struct SumJob : IJob
{
	[ReadOnly]
	public NativeArray<int> values;

	[WriteOnly]
	public NativeArray<int> result;


	public void Execute()
	{
		var len = values.Length;
		int sum = 0;
		for (int i = 0; i < len; ++i)
		{
			sum += values[i];
		}

		result[0] = sum;

	}
}
