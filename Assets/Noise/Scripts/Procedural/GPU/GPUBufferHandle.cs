using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procedural.GPU
{
	[System.Serializable]
	public struct GPUBufferHandle
	{
		public RenderTexture RenderTexture { get; private set; }

		public int Width  { get; private set; }
		public int Height { get; private set; }

		public bool IsCreated => isCreated && RenderTexture != null;
		private bool isCreated;

		private bool isTemp;

		/// <summary>
		/// Theoretical range of values in the buffer. The actual range could be different, this one originates from the fact that the noise nodes generate values in the theoretical range of [0, 1].
		/// </summary>
		public float2 Range { get; set; }


		public GPUBufferHandle(int width, int height)
		{
			RenderTexture = null;
			Width = -1;
			Height = -1;
			isCreated = false;
			Range = -1f;

			isTemp = false;

			CreateInternal(width, height);
		}


		private void CreateInternal(int width, int height)
		{
			this.Width = width;
			this.Height = height;
			RenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat);
			RenderTexture.Create();

			isCreated = true;
		}

		public void Clear(float val) => Clear(new Color(val, val, val, val));

		public void Clear(Color color)
		{
			var oldRt = RenderTexture.active;
			RenderTexture.active = this;
			GL.Clear(false, true, color);
			RenderTexture.active = oldRt;
		}

		public void Resize(int width, int height)
		{
			if (width == this.Width && height == this.Height)
				return;

			if (IsCreated)
				Release();

			CreateInternal(width, height);
		}

		public void Release()
		{
			Width = -1;
			Height = -1;

			Range = -1f;

			RenderTexture?.Release();
			isCreated = false;
		}

		public void ReleaseIfTemp()
		{
			if (isTemp)
			{
				this.Release();
			}
		}

		public AsyncGPUReadbackRequest RequestIntoNativeArray(ref NativeArray<float> heightmap,
		                                                      Action<AsyncGPUReadbackRequest> callback)
		{
			return AsyncGPUReadback.RequestIntoNativeArray(ref heightmap, RenderTexture, 0, callback);
		}

		public static GPUBufferHandle GetTemp(int width, int height)
		{
			var rt = UnityEngine.RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8);
			var buffer = new GPUBufferHandle
			             {
				             Width = width, Height = height, RenderTexture = rt, isTemp = true, Range = 0f,
				             isCreated = true
			             };
			return buffer;
		}

		public static GPUBufferHandle Create(object obj)
		{
			switch (obj)
			{
				case GPUBufferHandle buffer:
					return buffer;
				case float f:
				{
					var buffer = GetTemp(1, 1);
					buffer.Clear(f);
					return buffer;
				}
				case bool b:
				{
					var buffer = GetTemp(1, 1);
					buffer.Clear(b ? 1f : 0f);
					return buffer;
				}
				default:
					throw new Exception($"Cannot create GPUBufferHandle from type {obj.GetType()}");
			}
		}

		public static implicit operator RenderTexture(GPUBufferHandle handle)
		{
			return handle.RenderTexture;
		}

		public static implicit operator RenderTargetIdentifier(GPUBufferHandle handle)
		{
			return handle.RenderTexture;
		}


		//
		// public static implicit operator GPUBufferHandle(float val)
		// {
		// 	var buf = new GPUBufferHandle(1, 1);
		// 	buf.Clear(val);
		// 	return buf;
		// }
	}
}
