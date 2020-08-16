using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class MapGenerator
{

	public float[,] heightMap { get; }

	public NativeArray<float> heightmapNative;

	public int width;
	public int height;

	public MapGenerator(float[,] heightMap)
	{
		this.heightMap = heightMap;
	}

	public MapGenerator(NativeArray<float> heightmapNative, int width, int height)
	{
		this.heightmapNative = heightmapNative;
		this.width = width;
		this.height = height;
	}

	public Mesh GetMeshNative(float heightAmplitude, float unitSize = 1f)
	{

		if (width == 0 || height == 0 || heightmapNative.IsCreated == false)
			throw new System.Exception("Native heightmap is not ready!");

		int squareCount = (width - 1) * (height - 1);
		int triCount = squareCount * 2;
		int indexCount = triCount * 3;
		var indicesNative = new NativeArray<uint>(indexCount, Allocator.TempJob);

		var indicesJob = new GridIndicesJob
		{
			width = (uint)width,
			height = (uint)height,
			indices = indicesNative,
		};

		var indicesJobHandle = indicesJob.Schedule(squareCount, 8);

		int vertexCount = width * height;

		var verticesNative = new NativeArray<float3>(vertexCount, Allocator.TempJob);

		var verticesJob = new TerrainVerticesFromHeightmapJob
		{
			width = width,
			height = height,
			unitSize = unitSize,
			heightAmplitude = heightAmplitude,
			heightmap = heightmapNative,
			vertices = verticesNative
		};

		var verticesJobHandle = verticesJob.Schedule(width * height, 8);

		Mesh mesh = new Mesh();

		mesh.SetVertexBufferParams(vertexCount, new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3));
		mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

		verticesJobHandle.Complete();
		indicesJobHandle.Complete();


		mesh.SetVertexBufferData(verticesNative, 0, 0, verticesNative.Length);

		mesh.SetIndexBufferData(indicesNative, 0, 0, indicesNative.Length, MeshUpdateFlags.Default);

		mesh.SetSubMesh(0, new SubMeshDescriptor(0, indicesNative.Length, MeshTopology.Triangles));


		mesh.RecalculateNormals();

		mesh.RecalculateBounds();

		//mesh.Optimize();

		return mesh;
	}

	public Mesh GetMesh(float maxHeight, float unitSize = 1f)
	{
		Mesh mesh = new Mesh();

		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);

		int squareCount = (width - 1) * (height - 1);
		int triCount = squareCount * 2;
		int indexCount = triCount * 3;

		Vector3[] vertices = new Vector3[width * height];
		int[] indices = new int[indexCount];

		int triPass = 0;

		for (int i = 0; i < height; i++)
		{
			float z = i * unitSize;
			for (int j = 0; j < width; j++)
			{
				float y = heightMap[j, i] * maxHeight;

				float x = j * unitSize;

				var p = new Vector3(x, y, z);

				int index = i * width + j;

				vertices[index] = p;



				if (j < width - 1 && i < height - 1)
				{
					indices[triPass * 6 + 0] = index + 1 + width;
					indices[triPass * 6 + 1] = index + 1;
					indices[triPass * 6 + 2] = index;
					indices[triPass * 6 + 3] = index + width;
					indices[triPass * 6 + 4] = index + 1 + width;
					indices[triPass * 6 + 5] = index;

					triPass++;
				}

			}
		}

		mesh.vertices = vertices;
		mesh.triangles = indices;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		mesh.Optimize();

		return mesh;
	}


}


[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
public struct TerrainVerticesFromHeightmapJob : IJobParallelFor
{

	[ReadOnly]
	public float unitSize;

	[ReadOnly]
	public float heightAmplitude;

	[ReadOnly]
	public int width;
	[ReadOnly]
	public int height;

	[ReadOnly]
	public NativeArray<float> heightmap;

	[WriteOnly]
	public NativeArray<float3> vertices;

	public void Execute(int index)
	{
		int xi = (index / height);
		int yi = (index % height);
		float x = xi * unitSize;
		float z = yi * unitSize;

		float y = heightmap[index] * heightAmplitude;

		vertices[index] = new float3(x, y, z);
	}

}


[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
public struct GridIndicesJob : IJobParallelFor
{

	[ReadOnly]
	public uint width;
	[ReadOnly]
	public uint height;

	[WriteOnly]
	[NativeDisableParallelForRestriction]
	public NativeArray<uint> indices;

	public void Execute(int index)
	{

		uint r = (uint)(index / (height - 1));

		uint i = (uint)index + r;

		indices[index * 6 + 0] = i + 1 + height;
		indices[index * 6 + 1] = i + height;
		indices[index * 6 + 2] = i;
		indices[index * 6 + 3] = i + 1;
		indices[index * 6 + 4] = i + 1 + height;
		indices[index * 6 + 5] = i;

	}

}
