using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.RuntimeCollections;
using UnityEngine;

namespace UnityCommon.ResourceManagement
{
	public static class ResourceManager
	{
		private static Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();

		private static Dictionary<Type, Pool> pools = new Dictionary<Type, Pool>();




		public static void SetupPool<T>(int capacity, PoolObjectFactory<T> factory, bool autoResize = true) where T : IPoolObject
		{
			var type = typeof(T);

			if (pools == null)
				pools = new Dictionary<Type, Pool>();

			if (pools.ContainsKey(type))
			{
				Debug.Log($"Resource Manager already contains a pool of type {type}");
				return;
			}

			var pool = new Pool<T>(capacity, factory, autoResize);

			pools[type] = pool;

		}


		public static T GetPoolObject<T>() where T : IPoolObject
		{
			var type = typeof(T);
			Pool pool;
			if (!pools.TryGetValue(type, out pool))
			{
				throw new KeyNotFoundException($"Resource Manager doesn't contain a pool of type {type}");
			}

			return ((Pool<T>)pool).Recycle();
		}

		public static void ReturnPoolObject<T>(T obj) where T : IPoolObject
		{
			var type = typeof(T);
			Pool pool;
			if (!pools.TryGetValue(type, out pool))
			{
				throw new KeyNotFoundException($"Tried to return a pool object, but Resource Manager doesn't contain a pool of type {type}");
			}

			((Pool<T>)pool).Return(obj);
		}


		public static void DisposePool<T>()
		{
			var type = typeof(T);
			Pool pool;
			if (!pools.TryGetValue(type, out pool))
			{
				throw new KeyNotFoundException($"Tried to dispose a pool, but Resource Manager doesn't contain a pool of type {type}");
			}

			pools.Remove(type);

			pool.Dispose();

			pool = null;
		}


		public static void DisposeAllPools()
		{
			if (pools == null)
				return;

			foreach (var val in pools.Values)
			{
				val.Dispose();
			}


			pools.Clear();
			pools = null;

		}


		public static void Preload() // TODO: Divide over frames (maybe)
		{
			resources = new Dictionary<string, UnityEngine.Object>(32);

			StringCollection preloadPaths = Resources.Load<StringCollection>("PreloadCache");

			if (preloadPaths == null)
			{
				Debug.Log("No cached preload path found.");
				return;
			}

			foreach (var path in preloadPaths)
			{
				var obj = Resources.Load(string.Format("Preload/{0}", path));
				resources.Add(path, obj);
				//Debug.Log("Loaded resource: " + path);
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">Relative to "Resources/Preload" folder</param>
		/// <returns>Resource as UnityEngine.Object</returns>
		public static UnityEngine.Object GetResource(string path)
		{
			UnityEngine.Object obj = null;
			resources.TryGetValue(path, out obj);
			return obj;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">Relative to "Resources/Preload" folder</param>
		/// <returns>Resource as T</returns>
		public static T GetResource<T>(string path) where T : UnityEngine.Object
		{
			var obj = GetResource(path);
			if (obj == null)
				return default;

			return (T)obj;
		}

		public static GameObject Instantiate(string path)
		{
			var obj = GetResource<GameObject>(path);

			if (obj == null)
			{
				throw new MissingReferenceException($"Tried to instantiate resource at '{path}' but it doesn't exist");
			}

			return GameObject.Instantiate(obj);

		}


	}
}
