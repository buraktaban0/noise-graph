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
	public class ImageAlphaBinder : UIBinder<Image>
	{
		public FloatVariable variable;

		public FloatReference maxValue;


		private void OnEnable()
		{
			variable.OnModified.AddListener(OnVariableChanged);
		}

		private void OnDisable()
		{
			variable.OnModified.RemoveListener(OnVariableChanged);
		}


		public void OnVariableChanged(float val)
		{
			val = Mathf.Clamp01(val / maxValue.Value);
			var color = uiElement.color;
			color.a = val;
			uiElement.color = color;
		}

	}
}
