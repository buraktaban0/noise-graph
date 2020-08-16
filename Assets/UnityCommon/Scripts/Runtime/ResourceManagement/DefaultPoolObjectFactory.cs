using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.ResourceManagement
{

	public class DefaultPoolObjectFactory<T> : PoolObjectFactory<T> where T : IPoolObject, new()
	{

		public override T Produce()
		{
			return new T();
		}


		public override void Destroy(ref T obj)
		{
			obj = default;
		}


		public override void Dispose()
		{
			// Empty, nothing to dispose
		}

		public override void Reset()
		{

		}


	}

}
