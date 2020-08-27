using Noise.Runtime.Attributes;
using Procedural.GPU;

namespace Noise.Runtime.Nodes
{
	public class MasterNode : NoiseGraphNode
	{

		[Input]
		public GPUBufferHandle Final;


		public override bool ShouldCreateBuffer() => false;


		public override void Process()
		{
			// Do nothing, this is master
		}

	}
}
