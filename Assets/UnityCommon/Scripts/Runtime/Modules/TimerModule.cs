using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon.Modules
{
	[UnityEngine.CreateAssetMenu(fileName = "TimerModule", menuName = "Modules/Timer")]
	public class TimerModule : Module<TimerModule>
	{

		private List<TimedAction> actions;

		public TimedAction ScheduleAction(Action action, float delay, float period)
		{
			TimedAction timedAction = new TimedAction(action, delay, period);

			for (int i = 0; i < actions.Count; i++)
			{
				var act = actions[i];
				if (act.countdown > delay)
				{
					actions.Insert(i, timedAction);
					return timedAction;
				}
			}

			actions.Add(timedAction);
			return timedAction;

		}

		public void CancelAction(TimedAction timedAction)
		{
			if (actions.Contains(timedAction))
			{
				actions.Remove(timedAction);
			}
		}



		public override void OnStart()
		{
			actions = new List<TimedAction>(8);
		}

		public override void OnStop()
		{
			actions?.Clear();
			actions = null;
		}

		public override void OnUpdate()
		{

			float dt = UnityEngine.Time.deltaTime;
			int index = 0;
			for (int i = 0; i < actions.Count; i++, index++)
			{
				var act = actions[index];
				act.countdown -= dt;
				if (act.countdown <= 0)
				{
					act.countdown += act.period;
					act.action.Invoke();
					actions.RemoveAt(index);
					actions.Add(act);
					index--;
				}
			}

		}

		public override void OnLateUpdate()
		{

		}




	}

	public class TimedAction
	{
		public Action action;
		public float countdown;
		public float period;

		private float initialDelay;

		public TimedAction(Action action, float countdown, float period)
		{
			this.action = action;
			this.initialDelay = countdown;
			this.countdown = countdown;
			this.period = period;
		}


		public void Update(float dt)
		{
			countdown -= dt;

			while (countdown < 0f)
			{
				countdown += period;
				action.Invoke();
			}

		}


		public void Execute()
		{
			action.Invoke();
		}


		public void ResetToPeriod()
		{
			countdown = period;
		}

		public void ResetToInitialDelay()
		{
			countdown = initialDelay;
		}


	}


}
