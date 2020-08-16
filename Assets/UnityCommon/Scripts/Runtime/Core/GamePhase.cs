using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace UnityCommon.Core
{

	
	public abstract class GamePhase : ScriptableObject, IGamePhase
	{
		public object SharedData { get; internal set; }

		public bool IsInitialPhase { get; internal set; } = false;

		public bool IsRunning { get; set; }

		public virtual void OnStart()
		{
			IsRunning = true;
		}


		public abstract void OnUpdate();

		public abstract void OnLateUpdate();

		public abstract void OnFixedUpdate();


		public virtual void OnStop()
		{
			IsRunning = false;
		}

		public virtual void OnApplicationPause(bool pause)
		{

		}

		public abstract void OnApplicationQuit();


		public GamePhase Copy()
		{
			return Instantiate(this);
		}


		public static GamePhase Get(string name)
		{
			var gamePhase = Resources.FindObjectsOfTypeAll<GamePhase>().Where(phase => phase.name == name).FirstOrDefault();

			if (gamePhase == null)
			{
				throw new NullReferenceException("There is no GamePhase named \"" + name + "\"");
			}

			return gamePhase;
		}


		public static Dictionary<Type, GamePhase> LoadAll()
		{
			/*
			Stopwatch sw = new Stopwatch();
			sw.Start();
			*/

			var dict = new Dictionary<Type, GamePhase>(8);

			var gamePhaseType = typeof(GamePhase);

			var phases = Resources.LoadAll<GamePhase>("GamePhases");

			foreach (var phase in phases)
			{
				phase.hideFlags = HideFlags.DontUnloadUnusedAsset;
				DontDestroyOnLoad(phase);

				dict.Add(phase.GetType(), phase);
			}

			/*
			sw.Stop();
			UnityEngine.Debug.Log("GamePhases loaded in " + sw.Elapsed.TotalMilliseconds + " ms");
			*/

			return dict;

		}

		public static Dictionary<Type, GamePhase> LoadAllOLD()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			var dict = new Dictionary<Type, GamePhase>(8);

			var gamePhaseType = typeof(GamePhase);

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes().Where(t => t.BaseType == gamePhaseType && t.IsAbstract == false))
				{
					var gamePhase = Resources.LoadAll("GamePhases", type).FirstOrDefault();

					if (gamePhase != null)
					{
						gamePhase.hideFlags = HideFlags.DontUnloadUnusedAsset;
						DontDestroyOnLoad(gamePhase);

						dict.Add(type, (GamePhase)gamePhase);
					}

				}
			}


			sw.Stop();
			UnityEngine.Debug.Log("GamePhases loaded in " + sw.Elapsed.TotalMilliseconds + " ms");

			return dict;

		}




	}


}
