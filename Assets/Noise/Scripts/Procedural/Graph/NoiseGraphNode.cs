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
		protected static readonly int HASH_TEXA = Shader.PropertyToID("_TexA");
		protected static readonly int HASH_TEXB = Shader.PropertyToID("_TexB");
		protected static readonly int HASH_FACTOR = Shader.PropertyToID("_Factor");
		protected static readonly int HASH_RANGE = Shader.PropertyToID("_Range");
		protected static readonly int HASH_MAP = Shader.PropertyToID("_Map");
		protected static readonly int HASH_CURVEDATA = Shader.PropertyToID("_CurveData");
		protected static readonly int HASH_SAMPLECOUNT = Shader.PropertyToID("_SampleCount");
		protected static readonly int HASH_PERM = Shader.PropertyToID("p");

		protected NoiseGraph noiseGraph;

		private GPUNoiseBufferHandle buffer;
		public ref GPUNoiseBufferHandle Buffer
		{
			get
			{
				ValidateBuffer();

				return ref buffer;
			}
		}

		protected bool isClear = false;

		protected override void Init()
		{
			base.Init();

			noiseGraph = graph as NoiseGraph;

			ValidateBuffer();
		}

		public virtual bool GetIsDirty()
		{
			return isDirty || Ports.Where(p => p.IsInput).Any(p => p.node != null && ((NoiseGraphNode)(p.node)).GetIsDirty());
		}


		public virtual bool IsMasterNode() => false;

		public virtual bool ShouldCreateBuffer() => true;

		public virtual Color GetClearColor() => new Color(0f, 0f, 0f, 0f);

		public virtual int GetBufferSize() => noiseGraph.width;




		public virtual void ClearBuffer()
		{
			if (isClear)
				return;

			var defaultRenderTarget = RenderTexture.active;
			Graphics.SetRenderTarget(Buffer);
			GL.Clear(false, true, GetClearColor(), 0f);
			Graphics.SetRenderTarget(defaultRenderTarget);

			Buffer.Range = GetClearColor().r;

			isClear = true;
		}

		public GPUNoiseBufferHandle GetBuffer()
		{
			ValidateBuffer();
			return Buffer;
		}

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
