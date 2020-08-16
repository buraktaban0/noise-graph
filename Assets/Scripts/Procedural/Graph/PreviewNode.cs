using System.Collections;
using System.Collections.Generic;
using Procedural.Jobs;
using Procedural.Native;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;
using System;
using Procedural.GPU;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Procedural.Graph
{

	public class PreviewNode : NoiseGraphNode
	{

		[Input] public GPUNoiseBufferHandle a;
		[Input] public bool update = true;

		[Output] public GPUNoiseBufferHandle output;

		[HideInInspector]
		public Texture2D previewTexture;

		[HideInInspector]
		public readonly int previewTextureSize = 128;

		protected override void Init()
		{
			base.Init();

		}
		

		public override int GetBufferSize() => previewTextureSize;

		public override Color GetClearColor() => Color.magenta * 0.5f;


		public override object GetValue(NodePort port)
		{

			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUNoiseBufferHandle));


			if (a.IsCreated == false)
			{
				ClearBuffer();
				return a;
			}

			if (update)
			{
				Graphics.Blit(a, buffer);
				isClear = false;
			}

			return a;
		}


	}

}
