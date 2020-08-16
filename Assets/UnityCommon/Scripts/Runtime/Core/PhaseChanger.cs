using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Core;
using UnityEngine;

namespace UnityCommon.Runtime.Core
{

	public class PhaseChanger : MonoBehaviour
	{

		[SerializeField] private GamePhase phase = null;

		[SerializeField] private bool noTransition = true;

		public void ChangePhase()
		{
			ChangePhase(phase);
		}

		public void ChangePhase(GamePhase phase)
		{
			GameManager.Instance.SetPhase(phase, noTransition);
		}

	}

}
