using System.Collections;
using System.Collections.Generic;
using Procedural.GPU;
using Procedural.Native;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{

	public class MinMaxNode : NoiseGraphNode
	{
		private readonly string shaderName = "Hidden/Procedural/Multiply";

		[Input] public GPUNoiseBufferHandle a;

		[Output] public float2 result;

		private ComputeShader cs;

		private int kernelIndex;

		public override bool ShouldCreateBuffer() => false;

		public override object GetValue(NodePort port)
		{
			didExecute = true;

			ValidateBuffer();

			var a = GetInputValue("a", default(GPUNoiseBufferHandle));

			if (a.IsCreated == false)
			{
				return 0f;
			}

			if (cs == null)
			{
				cs = Resources.Load<ComputeShader>("CS_MinMax");
				kernelIndex = cs.FindKernel("CSMain");
			}

			var buffer = new ComputeBuffer(1, 8, ComputeBufferType.Structured);
			buffer.SetData(new float2[] { -2f });

			cs.SetTexture(kernelIndex, "_Tex", a);
			cs.SetBuffer(kernelIndex, "_Bounds", buffer);

			cs.Dispatch(kernelIndex, a.Width / 8, a.Width / 8, 1);

			float2[] arr = new float2[1];
			buffer.GetData(arr);

			float2 minMax = arr[0];

			return minMax;
		}


	}

}
