using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon.Modules
{

	[UnityEngine.CreateAssetMenu(fileName = "ConditionalsModule", menuName = "Modules/Conditionals")]
	public class ConditionalsModule : Module<ConditionalsModule>
	{
		internal List<Conditional> conditionals;


		public override void OnStart()
		{
			conditionals = new List<Conditional>(8);
		}

		public override void OnStop()
		{
			conditionals?.Clear();
			conditionals = null;
		}

		public override void OnUpdate()
		{
			for (int i = conditionals.Count - 1; i >= 0; i--)
			{
				try
				{
					conditionals[i].Update();
					if (conditionals[i].IsDone)
						conditionals.RemoveAt(i);
				}
				catch (Exception ex)
				{
					if (conditionals[i].Suppress)
					{
						UnityEngine.Debug.Log($"Exception encountered in conditional, removing chain: {ex.ToString()}");
					}
					else
					{
						UnityEngine.Debug.LogError($"Exception encountered in conditional, removing chain: {ex.ToString()}");
					}
					conditionals.RemoveAt(i);
				}

			}
		}


		public override void OnLateUpdate()
		{

		}



	}


	[System.Serializable]
	public abstract class Conditional
	{


		protected Func<bool> cond, cancelCond;
		protected Action act;

		private Conditional next;

		private bool suppress = false;
		internal bool Suppress => suppress;

		private bool isDone = false;
		internal bool IsDone
		{
			get => isDone;
			set
			{
				isDone = value;

				if (isDone && next != null)
				{
					next.Start();
					ConditionalsModule.Instance.conditionals.Add(next);
				}
			}
		}

		public abstract void Start();

		public abstract void Update();


		public void SuppressExceptions(bool val = true)
		{
			suppress = val;
		}

		public void Cancel()
		{
			IsDone = true;
		}


		public Conditional Do(Action act)
		{
			this.act = act;
			return this;
		}


		public Conditional CancelIf(Func<bool> func)
		{
			cancelCond = func;
			return this;
		}


		public ConditionalContinuous ThenWhile(Func<bool> func)
		{
			var cond = new ConditionalContinuous();
			cond.cond = func;
			this.next = cond;
			return cond;
		}

		public ConditionalOnce ThenIf(Func<bool> func)
		{
			var cond = new ConditionalOnce();
			cond.cond = func;
			this.next = cond;
			return cond;
		}

		public ConditionalWait ThenWait(float seconds)
		{
			var cond = new ConditionalWait(seconds);
			this.next = cond;
			return cond;
		}


		public static ConditionalContinuous While(Func<bool> func)
		{
			var cond = new ConditionalContinuous();
			cond.cond = func;
			ConditionalsModule.Instance.conditionals.Add(cond);
			return cond;
		}

		public static ConditionalOnce If(Func<bool> func)
		{
			var cond = new ConditionalOnce();
			cond.cond = func;
			ConditionalsModule.Instance.conditionals.Add(cond);
			return cond;
		}

		public static ConditionalWait Wait(float seconds)
		{
			var cond = new ConditionalWait(seconds);
			ConditionalsModule.Instance.conditionals.Add(cond);
			cond.Start();
			return cond;
		}




	}


	public class ConditionalOnce : Conditional
	{
		public override void Start()
		{

		}

		public override void Update()
		{
			if (cancelCond != null && cancelCond.Invoke())
			{
				IsDone = true;
				return;
			}

			if (cond.Invoke())
			{
				act?.Invoke();
				IsDone = true;
			}

		}

	}

	public class ConditionalContinuous : Conditional
	{
		public override void Start()
		{

		}

		public override void Update()
		{
			if (cancelCond != null && cancelCond.Invoke())
			{
				IsDone = true;
				return;
			}

			if (cond.Invoke())
			{
				act?.Invoke();
			}
		}
	}

	public class ConditionalWait : Conditional
	{
		private float delay;
		private float timer;
		private float endTime;

		private bool useUnscaledTime = false;

		public ConditionalWait(float delay)
		{
			this.delay = delay;
		}

		public override void Start()
		{
			timer = 0f;
		}

		public ConditionalWait UnscaledTime(bool enabled = true)
		{
			useUnscaledTime = enabled;
			return this;
		}

		public override void Update()
		{
			timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

			if (timer >= delay)
			{
				act?.Invoke();
				IsDone = true;
			}
		}




	}


}
