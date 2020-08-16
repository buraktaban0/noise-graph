using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon.Runtime.UI.DataBinding
{

	[RequireComponent(typeof(Toggle))]
	public class TwoWayToggleBinder : MonoBehaviour
	{

		[SerializeField] [HideInInspector] private Toggle toggle = null;

		[SerializeField] private BoolVariable variable = null;

		private bool justModifiedInternally = false;

#if UNITY_EDITOR
		private void OnValidate()
		{
			toggle = GetComponent<Toggle>();
		}
#endif


		private void Awake()
		{

			toggle.isOn = variable.Value;

			variable.OnModified.AddListener(OnValueModified);

			toggle.onValueChanged.AddListener(OnValueModifiedInternally);

		}

		private void OnValueModifiedInternally(bool val)
		{
			justModifiedInternally = true;

			variable.Value = val;
		}

		private void OnValueModified(bool val)
		{

			if (justModifiedInternally)
			{
				justModifiedInternally = false;
				return;
			}

			toggle.isOn = val;

		}



	}

}
