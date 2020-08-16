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

	public class TextBinderInt : TextBinder<IntReference, IntVariable, int>
	{

		[SerializeField] private int offset = 0;


		protected override void OnDataModified(int val)
		{
			val += offset;

			base.OnDataModified(val);
		}


	}


	/*

	public class TextBinderInt : UIBinder<Text>
	{

		[SerializeField] private IntVariable variable;


		public IntVariable Variable
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

			}
		}



		private void OnEnable()
		{

			if (variable != null)
			{
				OnVariableChanged(variable.Value);

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


		[TextArea]
		public string format = "";

		public void OnVariableChanged(int val)
		{

			var format = this.format.Trim();

			if (format == null || format.Length < 1)
			{
				element.text = val.ToString();
				return;
			}

			var str = val.ToString(format);
			element.text = str; //string.Format(format, val);

		}

	}

	*/

}
