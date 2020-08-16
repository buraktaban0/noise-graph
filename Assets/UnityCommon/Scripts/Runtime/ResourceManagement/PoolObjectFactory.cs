using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.ResourceManagement
{
	public abstract class PoolObjectFactory<T> : ObjectFactory<T> where T : IPoolObject
	{
		public abstract override T Produce();

		public abstract override void Destroy(ref T obj);

		public abstract override void Dispose();

		public abstract void Reset();

	}
}
