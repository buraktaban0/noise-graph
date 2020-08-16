using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procedural.GPU;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class NoiseGraphNode : Node
	{
		protected readonly int hashTexA = Shader.PropertyToID("_TexA");
		protected readonly int hashTexB = Shader.PropertyToID("_TexB");

		protected NoiseGraph noiseGraph;

		protected GPUNoiseBufferHandle buffer;

		protected bool isClear = false;

		protected override void Init()
		{
			base.Init();

			noiseGraph = graph as NoiseGraph;

			ValidateBuffer();

		}

		public virtual void ClearBuffer()
		{
			if (isClear)
				return;

			var defaultRenderTarget = RenderTexture.active;
			Graphics.SetRenderTarget(buffer);
			GL.Clear(false, true, GetClearColor(), 0f);
			Graphics.SetRenderTarget(defaultRenderTarget);

			buffer.Range = GetClearColor().r;

			isClear = true;
		}

		public GPUNoiseBufferHandle GetBuffer()
		{
			ValidateBuffer();
			return buffer;
		}

		public virtual bool ShouldCreateBuffer() => true;

		public virtual Color GetClearColor() => new Color(0f, 0f, 0f, 0f);

		public virtual int GetBufferSize() => noiseGraph.width;
		

		public void ValidateBuffer()
		{
			if (ShouldCreateBuffer())
			{
				int size = GetBufferSize();
				if (buffer.IsCreated == false || buffer.Width != size || buffer.Height != size)
				{
					if (buffer.IsCreated)
						buffer.Release();

					buffer = new GPUNoiseBufferHandle(size, size);
				}
			}
			else
			{
				if (buffer.IsCreated)
					buffer.Release();
			}

		}


	}

}
