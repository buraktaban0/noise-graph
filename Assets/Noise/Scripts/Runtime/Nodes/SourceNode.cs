using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using Procedural.GPU;
using UnityEngine;

namespace Noise.Runtime.Nodes
{

	public class SourceNode : NoiseGraphNode
	{

		private static readonly int BUFFER_SIZE = 512;

		[Output]
		public GPUNoiseBufferHandle Out;


		public override void Process()
		{
			base.Process();

			buffer.Clear(Color.gray);

			SetOutput("Out", buffer);

		}

	}

}
