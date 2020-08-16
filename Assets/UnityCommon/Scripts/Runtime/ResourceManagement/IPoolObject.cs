using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.ResourceManagement
{
	public interface IPoolObject
	{

		bool IsInUse { get; set; }

		void SetOwnerPool(Pool pool);

		void ReturnToPool();


		void CachePools();


		void OnPooled();
		void OnRecycled();


	}
}
