using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon.ResourceManagement
{
	public class GameObjectFactory<T> : PoolObjectFactory<T> where T : MonoBehaviour, IPoolObject
	{

		private GameObject prefab = null;

		private Transform root = null;

		private List<T> objs = new List<T>(128);


		public GameObjectFactory(GameObject prefab)
		{
			this.prefab = prefab;

			//root = new GameObject($"Pool_{prefab.name}").transform;

			//GameObject.DontDestroyOnLoad(root.gameObject);

		}

		public override T Produce()
		{
			var obj = GameObject.Instantiate(prefab);


			GameObject.DontDestroyOnLoad(obj);

			//obj.transform.parent = root;

			var comp = obj.GetComponent<T>();

			objs.Add(comp);

			return comp;
		}


		public override void Reset()
		{

			foreach (var obj in objs)
			{
				obj.transform.parent = null;
				obj.OnPooled();
				GameObject.DontDestroyOnLoad(obj.gameObject);
			}

		}


		public override void Destroy(ref T obj)
		{

			GameObject.Destroy(obj.gameObject);
			obj = null;
		}

		public override void Dispose()
		{
			if (root != null && root.gameObject != null)
				GameObject.Destroy(root.gameObject);



		}


	}
}
