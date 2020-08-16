using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityCommon.Core.PhaseTransitions
{

	public abstract class GamePhaseTransition : ScriptableObject
	{

		internal bool IsRunning { get; private set; } = false;

		private Action changePhaseAction;

		internal void SetPhaseAction(Action action)
		{
			changePhaseAction = action ?? throw new ArgumentNullException("Phase action cannot be null!");
		}

		public virtual void InitiateTransition()
		{
			IsRunning = true;

		}


		protected void CompleteTransition()
		{
			IsRunning = false;
			changePhaseAction.Invoke();
		}


	}

}
