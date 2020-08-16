using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon.Singletons
{
	public static class SingletonBehaviour
	{

		public static T GetInstance<T>() where T : Behaviour
		{
			var instance = GameObject.FindObjectOfType<T>();

			if (instance == null)
			{
				var obj = new GameObject(typeof(T) + "_Instance");
				GameObject.DontDestroyOnLoad(obj);
				instance = obj.AddComponent<T>();
			}

			return instance;
		}

	}

	public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
	{

		protected static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<T>();

					if (instance == null)
					{
						Debug.Log("Instance of type " + typeof(T) + " was not existent, creating new");

						var obj = new GameObject(typeof(T) + "_Instance");
						DontDestroyOnLoad(obj);
						instance = obj.AddComponent<T>();
					}
				}

				return instance;
			}

			protected set
			{
				instance = value;
			}
		}


		protected bool SetupInstance()
		{

			if (instance != null)
			{
				// Another instance present

				Debug.Log($"An instance of type {this.GetType()} already exists. Destroying duplicate.");

				Destroy(this.gameObject);
				return false;
			}


			instance = (T)this;
			DontDestroyOnLoad(this.gameObject);

			return true;

		}



	}


}
