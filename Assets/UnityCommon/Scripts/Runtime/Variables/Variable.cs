using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Events;
using UnityCommon.Singletons;
using UnityEngine;
using UnityEngine.Events;

namespace UnityCommon.Variables
{

	public abstract class Variable : ScriptableObject
	{
		private static Dictionary<string, Variable> variables;
		private static List<Variable> prefsVariables;

		[HideInInspector] [SerializeField] private bool bindToPlayerPrefs = false;
		public bool BindToPlayerPrefs
		{
			get => bindToPlayerPrefs;
		}

		[HideInInspector] [SerializeField] public string PrefsKey => $"Variable_{name}";



		public abstract object GetValueAsObject();


		public abstract void SetValueAsObject(object obj);

		public abstract void InvokeModified();

		public abstract void ResetToEditorValue();

		public virtual string Serialize()
		{
			UnityEngine.Debug.Log("Base Serialize");
			return GetValueAsObject().ToString();
		}

		public virtual void Deserialize(string s)
		{
			var val = Convert.ChangeType(s, GetType());
			SetValueAsObject(val);
		}

		public virtual bool CanBeBoundToPlayerPrefs() => true;


		public virtual void OnInspectorChanged()
		{

		}

		public abstract void RaiseModifiedEvent();

		public static void LoadAll()
		{

			if (prefsVariables != null)
			{
				foreach (var var in prefsVariables)
				{
					var.ResetToEditorValue();
				}
			}

			variables = new Dictionary<string, Variable>(16);
			prefsVariables = new List<Variable>(8);

			var loadedVariables = Resources.LoadAll<Variable>("Variables");

			for (int i = 0; i < loadedVariables.Length; i++)
			{
				var variable = loadedVariables[i];
				variables.Add(variable.name, variable);

				variable.hideFlags = HideFlags.DontUnloadUnusedAsset;
				DontDestroyOnLoad(variable);

				if (variable.BindToPlayerPrefs)
				{
					try
					{
						prefsVariables.Add(variable);

						var key = variable.PrefsKey;
						if (PlayerPrefs.HasKey(key))
						{

							var strVal = PlayerPrefs.GetString(key);
							variable.Deserialize(strVal);

						}
						else
						{
							variable.ResetToEditorValue();
						}
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogError(ex);
					}

				}
				else
				{
					variable.ResetToEditorValue();
				}

				variable.RaiseModifiedEvent();

			} // For end

		}



		public static void SavePrefsVariables()
		{
			foreach (var v in prefsVariables)
			{
				var key = v.PrefsKey;
				var value = v.Serialize();
				PlayerPrefs.SetString(key, value);
			}

			PlayerPrefs.Save();
		}


		public static T Get<T>(string name) where T : Variable
		{
			if (variables.ContainsKey(name) == false)
			{
				throw new KeyNotFoundException($"Variable with name {name} is not loaded.");
			}

			return (T)variables[name];
		}


		public static T Create<T>() where T : Variable
		{
			return ScriptableObject.CreateInstance<T>();
		}


	}

	public abstract class Variable<T> : Variable
	{




		[SerializeField] private T editorValue;

		[SerializeField] [HideInInspector] protected T value;
		public T Value
		{
			get { return value; }
			set
			{
				if (object.Equals(this.value, value))
				{
					return;
				}

				this.value = value;

				if (BindToPlayerPrefs)
				{
					PlayerPrefs.SetString(PrefsKey, this.Serialize());
				}

				OnModified.Raise(this, value);

			}
		}


		public GameEvent<T> OnModified { get; private set; }

		public override void RaiseModifiedEvent()
		{
			OnModified.Raise(this, Value);
		}

		private void OnEnable()
		{
			OnModified = new GameEvent<T>();

			if (!BindToPlayerPrefs)
				value = editorValue;

		}

		/*
		private void OnDisable()
		{
			OnModified = new GameEvent<T>();

			if (!BindToPlayerPrefs)
				value = editorValue;
		}
		*/

		public override object GetValueAsObject()
		{
			return value;
		}

		public override void SetValueAsObject(object obj)
		{
			value = (T)obj;
		}


		public override void InvokeModified()
		{
			editorValue = value;

			OnModified?.Raise(this, value);
		}


		public override void ResetToEditorValue()
		{
			Value = editorValue;
		}




		public static implicit operator T(Variable<T> v)
		{
			return v.Value;
		}



	}
}
