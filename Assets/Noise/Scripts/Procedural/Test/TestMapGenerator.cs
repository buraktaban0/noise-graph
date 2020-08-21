using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaughtyAttributes;
using Procedural;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class TestMapGenerator : MonoBehaviour
{
	public HeightMapGenerator.Settings settings;

	public LayeredNoise layeredNoise;


	private void Update()
	{
		if (layeredNoise.isModified)
		{
			layeredNoise.isModified = false;

			Generate();
		}
	}

	[Button]
	public void Generate()
	{

		var meshFilter = this.GetComponent<MeshFilter>();
		if (!meshFilter)
		{
			meshFilter = this.gameObject.AddComponent<MeshFilter>();
		}


		var meshRenderer = this.GetComponent<MeshRenderer>();
		if (!meshRenderer)
		{
			meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		}


		var heightmap = layeredNoise.GetHeightmapNative(settings.width, settings.height, Unity.Collections.Allocator.Persistent);

		var mapGenerator = new MapGenerator(heightmap, settings.width, settings.height);

		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

		if (layeredNoise.logTime)
			sw.Start();

		var mesh = mapGenerator.GetMeshNative(10f, .5f);


		if (layeredNoise.logTime)
		{
			sw.Stop();
			Debug.Log($"Mesh generation took {sw.Elapsed.TotalMilliseconds.ToString("0.00")} ms");
		}


		meshFilter.sharedMesh = mesh;

	}

}
