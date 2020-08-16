using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon.ResourceManagement
{


	public abstract class Pool
	{
		public abstract void Dispose(bool bypassOnPool = false);

		public abstract void Reset();

		public abstract void CachePools();

	}


	public class Pool<T> : Pool where T : IPoolObject
	{

		private Queue<T> objects;

		private List<T> allObjects;


		private int capacity;


		private bool autoResize;


		private bool isDisposed = false;

		private PoolObjectFactory<T> factory;


		public Pool(int capacity, PoolObjectFactory<T> factory, bool autoResize = true)
		{
			this.capacity = capacity;
			this.factory = factory;
			this.autoResize = autoResize;

			objects = new Queue<T>(capacity);

			allObjects = new List<T>(capacity);

			Populate(capacity);


		}


		public override void CachePools()
		{
			foreach (var obj in allObjects)
			{
				obj.CachePools();
			}
		}

		public override void Reset()
		{
			foreach (var obj in allObjects)
			{
				if (obj == null)
				{
					Debug.Log("Null reference in pool of type " + typeof(T) + " !");
				}
				else
				{
					if (obj.IsInUse)
						obj.ReturnToPool();
				}
			}

			if (objects.Count < allObjects.Count)
			{
				Debug.LogError($"Leak in pool of type {typeof(T)}. {allObjects.Count - objects.Count} objects lost!");
			}


			factory.Reset();
		}



		private void Populate(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var obj = factory.Produce();
				obj.IsInUse = false;
				obj.SetOwnerPool(this);
				obj.OnPooled();
				objects.Enqueue(obj);
				allObjects.Add(obj);
			}
		}


		public T Recycle()
		{

			if (objects.Count < 1)
			{
				if (autoResize)
				{
					Populate(capacity);
					capacity = (int)(capacity * 1.5f + 1);
					Debug.Log($"Resized pool of type {typeof(T)}");
				}
				else
				{
					throw new IndexOutOfRangeException($"Pool of type {typeof(T)}: All in use. Consider setting 'autoResize = true'");
				}
			}

			var obj = objects.Dequeue();
			obj.IsInUse = true;
			obj.OnRecycled();
			return obj;

		}

		public void Return(T obj)
		{
			obj.IsInUse = false;
			obj.OnPooled();
			objects.Enqueue(obj);
		}


		public override void Dispose(bool bypassOnPooled = false)
		{

			if (isDisposed)
				return;

			isDisposed = true;

			for (int i = 0; i < allObjects.Count; i++)
			{
				var obj = allObjects[i];

				if (bypassOnPooled == false)
					obj?.OnPooled();
				factory.Destroy(ref obj);
				obj = default;
			}

			objects.Clear();
			objects = null;

			allObjects.Clear();
			allObjects = null;

			factory.Dispose();
			factory = null;


		}

	}
}
