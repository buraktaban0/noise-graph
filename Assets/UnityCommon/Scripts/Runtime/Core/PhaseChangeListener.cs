using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Core;
using UnityEngine;
using UnityEngine.Events;

namespace UnityCommon.Runtime.Core
{

	public class PhaseChangeListener : MonoBehaviour
	{

		public GamePhase phaseToWatch;

		[SerializeField] private UnityEvent onChanged = null;

		private void Start()
		{
			GameManager.Instance.RegisterPhaseChangedListener(OnPhaseChanged);

			OnPhaseChanged(GameManager.Instance.GetCurrentPhase());

		}

		private void OnPhaseChanged(object phaseObj)
		{
			var phase = (GamePhase)phaseObj;

			if (phaseToWatch == null || phaseToWatch.GetType() == phase.GetType())
			{
				onChanged.Invoke();
			}

		}

	}

}
