using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procedural.GPU;
using Procedural.Native;
using UnityEngine;
using XNode;

namespace Procedural.Graph
{
	public class NoiseMasterNode : Node
	{
		[Input] public GPUNoiseBufferHandle final;


		public override object GetValue(NodePort port)
		{
			didExecute = true;

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			var result = GetInputValue("final", default(GPUNoiseBufferHandle));


			sw.Stop();

			Debug.Log($"Elaped: {sw.Elapsed.TotalMilliseconds.ToString("0.00")} ms - {result.IsCreated}");

			//result.Dispose();

			return result;
		}

		public override void OnModified()
		{
			base.OnModified();
		}

	}
}
