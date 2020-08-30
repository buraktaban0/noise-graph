using Noise.Runtime.Util;
using Procedural.GPU;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor.Util
{
	public static class PreviewUtil
	{
		private const int PreviewRtSize = 512;

		private static RenderTexture previewRt =
			new RenderTexture(PreviewRtSize, PreviewRtSize, 0, RenderTextureFormat.ARGB32);

		public static VisualElement GetPreviewElement(object previewedValue, float maxSize)
		{
			switch (previewedValue)
			{
				case GPUBufferHandle buffer:
					if (previewRt == null)
						previewRt =
							new RenderTexture(PreviewRtSize, PreviewRtSize, 0, RenderTextureFormat.ARGB32);

					Graphics.Blit(buffer, previewRt, MaterialCache.GetMaterial(MaterialCache.Op.SingleToMultiChannel));
					var img = new Image()
					{
						image = previewRt
					};
					img.style.width = math.min(previewRt.width, maxSize);
					img.style.height = math.min(previewRt.height, maxSize);
					return img;
				case float f:
					return new Label(f.ToString("0.000"));
				case float2 f2:
					return new Label(f2.ToString());
				case float3 f3:
					return new Label(f3.ToString());
				case float4 f4:
					return new Label(f4.ToString());
				default:
					return default;
			}
		}
	}
}
