using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Singletons;
using UnityEngine;

namespace UnityCommon.Modules
{

	public interface IModule : IComparable<IModule>
	{
		int ExecutionOrder { get; }

		void OnStart();
		void OnStop();

		void OnUpdate();

		void OnLateUpdate();

		string GetName();

		void CacheInstance();
	}


	public abstract class Module
	{
		internal static List<IModule> LoadAllOLD()
		{

			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();


			var modules = new List<IModule>();
			var ssoType = typeof(SingletonScriptableObject<>);

			foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var t in assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule)) && t.IsAbstract == false))
				{
					var genericType = ssoType.MakeGenericType(t);
					var property = genericType.GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

					try
					{
						var module = (ScriptableObject)property.GetMethod.Invoke(null, null);

						module.hideFlags = HideFlags.DontUnloadUnusedAsset;
						ScriptableObject.DontDestroyOnLoad(module);

						var iModule = (IModule)module;
						if (iModule != null)
						{
							modules.Add(iModule);
						}
					}
					catch { }

				}
			}

			modules.Sort(new Comparison<IModule>((m1, m2) => m1.ExecutionOrder.CompareTo(m2.ExecutionOrder)));


			sw.Stop();

			Debug.Log("Modules loaded in " + sw.Elapsed.TotalMilliseconds + " milliseconds");


			return modules;
		}

		internal static List<IModule> LoadAll()
		{
			/*
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			*/

			var modules = Resources.LoadAll("Modules").Select(obj => (IModule)obj).ToList();

			foreach (var module in modules)
			{
				module.CacheInstance();
			}

			modules.Sort();

			/*
			sw.Stop();

			Debug.Log("Modules loaded in " + sw.Elapsed.TotalMilliseconds + " milliseconds");
			*/

			return modules;

		}

	}

	public abstract class Module<T> : SingletonScriptableObject<T>, IModule where T : Module<T>
	{
		[Range(-5, 5)] [SerializeField] private int executionOrder = 0;
		public int ExecutionOrder => executionOrder;

		private void OnEnable()
		{
			//OnStart();
		}

		private void OnDisable()
		{
			//OnStop();
		}


		public string GetName() => this.GetType().Name;

		public abstract void OnStart();
		public abstract void OnStop();

		public abstract void OnUpdate();

		public abstract void OnLateUpdate();

		public void CacheInstance()
		{
			Instance = (T)this;
			hideFlags = HideFlags.DontUnloadUnusedAsset;
			DontDestroyOnLoad(this);
		}


		public int CompareTo(IModule other)
		{
			return ExecutionOrder.CompareTo(other.ExecutionOrder);
		}

	}

}
