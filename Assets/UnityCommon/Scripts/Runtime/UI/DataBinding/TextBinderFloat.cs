using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon.UI.DataBinding
{

	public class TextBinderFloat : TextBinder<FloatReference, FloatVariable, float>
	{

	}


	/*
	public class TextBinderFloat : UIBinder<Text>
	{
		public FloatReference reference;

		public FloatVariable variable;


		[TextArea]
		public string format;


		private void OnEnable()
		{
			variable.OnModified.AddListener(OnVariableChanged);
			OnVariableChanged(variable.Value);
		}
		private void OnDisable()
		{
			variable.OnModified.RemoveListener(OnVariableChanged);
		}


		public void OnVariableChanged(float val)
		{
			var format = System.Text.RegularExpressions.Regex.Unescape(this.format.Trim());

			if (format == null || format.Length < 1)
			{
				element.text = val.ToString();
				return;
			}

			var str = val.ToString(format);
			element.text = str; // string.Format(format, val);

		}


	}

	*/
}
