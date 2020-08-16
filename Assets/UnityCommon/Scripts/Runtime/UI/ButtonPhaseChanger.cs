using System.Collections;
using System.Collections.Generic;
using UnityCommon.Core;
using UnityCommon.UI.DataBinding;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon.UI
{

	public class ButtonPhaseChanger : UIBinder<Button>
	{

		[SerializeField] private GamePhase targetPhase = null;

		private GameManager gameManager;


		private void OnEnable()
		{
			gameManager = GameManager.Instance;

			uiElement.onClick.AddListener(
				OnClick
				);
		}

		private void OnDisable()
		{
			uiElement.onClick.RemoveAllListeners();
		}


		private void OnClick()
		{
			if (targetPhase == null)
			{
				Debug.LogError("'targetPhase' is null, changing phase aborted");
			}

			gameManager.SetPhase(targetPhase);
		}


	}

}
