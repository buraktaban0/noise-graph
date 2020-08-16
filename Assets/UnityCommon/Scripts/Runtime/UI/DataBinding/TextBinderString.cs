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

	public class TextBinderString : TextBinder<StringReference, StringVariable, string>
	{

	}

	/*

	public class TextBinderString : UIBinder<Text>
	{

		public StringVariable variable;


		[TextArea]
		public string format;


		private void OnEnable()
		{
			variable.OnModified.AddListener(OnVariableChanged);
		}
		private void OnDisable()
		{
			variable.OnModified.RemoveListener(OnVariableChanged);
		}


		public void OnVariableChanged(string val)
		{
			var format = this.format.Trim();

			if (format == null || format.Length < 1)
			{
				element.text = val;
				return;
			}

			element.text = string.Format(format, val);

		}


	}

	*/



}
