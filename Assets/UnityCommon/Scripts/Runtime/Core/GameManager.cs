using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Core.PhaseTransitions;
using UnityCommon.Collections;
using UnityCommon.Modules;
using UnityCommon.ResourceManagement;
using UnityCommon.Singletons;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityCommon.Events;
using UnityEngine.EventSystems;

namespace UnityCommon.Core
{

	[DefaultExecutionOrder(-100)]
	public class GameManager : SingletonBehaviour<GameManager>
	{


		public ArgumentCollection CmdArguments { get; private set; }


		[SerializeField]
		private GamePhase initialGamePhase = null;

		[SerializeField]
		private GamePhaseTransition phaseTransition = null;

		[Space(8)]
		[SerializeField]
		private bool restartPhaseOnLoadScene = true;


		private GamePhase currentGamePhase = null;

		private ConcurrentQueue<Action> updateQueue = new ConcurrentQueue<Action>();

		private List<IModule> modules;

		private Dictionary<Type, GamePhase> gamePhases;

		//private bool isApplicationQuitting = false;



		private GameEvent onGamePhaseChanged;



		private void Awake()
		{

			if (!SetupInstance())
			{
				//Debug.Log($"An instance of type {this.GetType()} already exists, destroying self.");
				return;
			}


			Application.targetFrameRate = 60;

			SceneManager.sceneLoaded += OnSceneLoaded;

			//CmdArguments = new ArgumentCollection(Environment.GetCommandLineArgs());

			ResourceManager.Preload();

			Variable.LoadAll();


			modules = Module.LoadAll();
			gamePhases = GamePhase.LoadAll();


			foreach (var m in modules)
			{
				m.OnStart();
			}


			onGamePhaseChanged = ScriptableObject.CreateInstance<GameEvent>();



			if (initialGamePhase == null)
			{
				Debug.Log("Initial GamePhase is null.");
			}
			else
			{
				currentGamePhase = initialGamePhase.Copy();

				currentGamePhase.IsInitialPhase = true;

				currentGamePhase.OnStart();
			}


		}

		/*
		private void OnGUI()
		{
			//GUILayout.Label($"FPS: {(int)(1f / Time.smoothDeltaTime)}");
		}
		*/

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			PlayerPrefs.Save();

			if (restartPhaseOnLoadScene)
			{
				SetPhase(currentGamePhase);
			}

		}


		public void SetPhase(GamePhase phase, bool noTransition = false)
		{
			var type = phase.GetType();

			SetPhase(type, noTransition);
		}

		public void SetPhase<T>(bool noTransition = false) where T : GamePhase
		{
			var type = typeof(T);

			SetPhase(type, noTransition);
		}

		private bool isChangingPhase = false;

		public void SetPhase(Type type, bool noTransition = false)
		{

			if (isChangingPhase)
			{
				Debug.Log("Already in the process of changing phase, aborting...");
				return;
			}

			isChangingPhase = true;

			if (gamePhases.ContainsKey(type) == false)
			{
				throw new MissingReferenceException($"There is no GamePhase of type {type} in the project. Did you forget to create a resource instance?");
			}


			Action phaseAction = () =>
			{
				PlayerPrefs.Save();

				currentGamePhase.OnStop();

				var targetGamePhase = gamePhases[type].Copy(); // Copying to prevent overwriting default state of phase objects in the editor

				targetGamePhase.SharedData = currentGamePhase.SharedData;

				Destroy(currentGamePhase);

				currentGamePhase = targetGamePhase;
				currentGamePhase.OnStart();

				isChangingPhase = false;

				onGamePhaseChanged.Raise(this, currentGamePhase);

			};



			if (noTransition || this.phaseTransition == null)
			{
				phaseAction.Invoke();
				return;
			}


			phaseTransition.SetPhaseAction(phaseAction);
			phaseTransition.InitiateTransition();

		}


		public GamePhase GetCurrentPhase() => currentGamePhase;

		public T GetCurrentPhaseAs<T>() where T : GamePhase
		{
			return currentGamePhase as T;
		}

		public void RegisterPhaseChangedListener(Action<object> listener)
		{
			onGamePhaseChanged.AddListener(listener);
		}
		public void UnregisterPhaseChangedListener(Action<object> listener)
		{
			onGamePhaseChanged.RemoveListener(listener);
		}


		public void RunOnMainThread(Action a)
		{
			updateQueue.Enqueue(a);
		}



		/*
		private void OnEnable()
		{
			if (instance != null && instance != this)
			{
				Debug.Log("An instance already exists, disabling self");
				this.enabled = false;
				return;
			}

			instance = this;

			foreach (var m in modules)
				m.OnStart();


			//currentGamePhase?.OnStart();
		}

		private void OnDisable()
		{
			if (isApplicationQuitting)
				return;

			if (Instance != this)
				return;

			//currentGamePhase?.OnStop();

			foreach (var m in modules)
				m.OnStop();

			Instance = null;
		}
		*/

		private void FixedUpdate()
		{
			currentGamePhase?.OnFixedUpdate();
		}

		private void Update()
		{
			foreach (var m in modules)
			{
				m.OnUpdate();
			}

			Action a;
			while (updateQueue.TryDequeue(out a))
			{
				a?.Invoke();
			}

			currentGamePhase?.OnUpdate();

			//QualitySettings.vSyncCount = 0;
			//Application.targetFrameRate = 60;



		}


		private void LateUpdate()
		{
			foreach (var m in modules)
			{
				m.OnLateUpdate();
			}

		}

		private void OnApplicationPause(bool pause)
		{
			currentGamePhase?.OnApplicationPause(pause);
		}

		private void OnApplicationQuit()
		{
			//isApplicationQuitting = true;

			currentGamePhase?.OnApplicationQuit();

			foreach (var m in modules)
			{
				m.OnStop();
			}

		}


	}
}
