using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Nodes;

namespace Noise.Runtime
{

	public class NodeTreeElement
	{

		public NoiseGraphNode node;

		public bool wasEvaluated = false;

		public List<object> outputs;

		public void Reset()
		{
			wasEvaluated = false;
		}

		public void Evaluate()
		{
			wasEvaluated = true;
		}

	}

}
