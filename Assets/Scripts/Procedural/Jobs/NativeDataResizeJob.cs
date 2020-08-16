using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

namespace Procedural.Jobs
{
	public struct NativeDataResizeJob : IJobParallelFor
	{

		[ReadOnly]
		public int size0;

		[ReadOnly]
		public int size1;

		[ReadOnly]
		public NativeArray<float> a;

		[WriteOnly]
		public NativeArray<float> b;


		public void Execute(int i1)
		{
			int x1 = i1 / size1;
			int y1 = i1 % size1;

			int x0 = (int)((float)x1 * size0 / size1);
			int y0 = (int)((float)y1 * size0 / size1);

			int i0 = x0 * size0 + y0;

			b[i1] = a[i0];

		}

	}
}
