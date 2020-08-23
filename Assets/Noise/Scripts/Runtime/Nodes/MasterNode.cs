using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using Procedural.GPU;
using Unity.Mathematics;

namespace Noise.Runtime.Nodes
{
	public class MasterNode : NoiseGraphNode
	{

		[Input]
		public GPUNoiseBufferHandle Final;


		public override bool ShouldCreateBuffer() => false;


		public override void Process()
		{
			// Do nothing, this is master
		}

	}
}
