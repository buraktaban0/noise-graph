using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityCommon.Core.PhaseTransitions
{


	[CreateAssetMenu(menuName = "Phase Transitions/Instant", fileName = "InstantPhaseTransition")]
	public class InstantPhaseTransition : GamePhaseTransition
	{
		
		public override void InitiateTransition()
		{
			base.InitiateTransition();

			CompleteTransition();

		}
		

	}


}
