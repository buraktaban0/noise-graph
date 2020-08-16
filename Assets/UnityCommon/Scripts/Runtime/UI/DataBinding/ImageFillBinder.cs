using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Events;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon.UI.DataBinding
{
	[ExecuteInEditMode]
	public class ImageFillBinder : UIBinder<Image>
	{
		[SerializeField] private FloatVariable variable;

		public FloatReference maxValue;



		public FloatVariable Variable
		{
			get
			{
				return variable;
			}

			set
			{
				variable = value;

				if (value == null)
					return;


				if (this.enabled)
				{
					OnEnable();
				}

				OnVariableChanged(variable.Value);

			}
		}


		private void OnEnable()
		{
			if (variable != null)
			{
				variable.OnModified.AddListener(OnVariableChanged);
			}
		}

		private void OnDisable()
		{
			if (variable != null)
			{
				variable.OnModified.RemoveListener(OnVariableChanged);
			}
		}


		public void OnVariableChanged(float val)
		{
			uiElement.fillAmount = Mathf.Clamp01(val / maxValue.Value);
		}

	}
}
