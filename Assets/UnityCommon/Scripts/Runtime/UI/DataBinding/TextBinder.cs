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

	[DisallowMultipleComponent]
	public class TextBinder<TRef, TVar, T> : UIBinder<Text> where TRef : Reference<T, TVar>, new() where TVar : Variable<T>
	{

		[SerializeField] private TRef data;


		[SerializeField] [TextArea] private string format = "";

		private bool isInitialized = false;


		private void Reset()
		{
			data = new TRef();
			data.type = Reference.Type.GlobalVariable;
		}

		protected override void OnValidate()
		{
			base.OnValidate();


			if (data.type == Reference.Type.Constant)
			{
				Debug.Log("Observed data can not be 'Constant', switching to 'Global'");
				data.type = Reference.Type.GlobalVariable;
			}

		}


		protected virtual void Start()
		{

			isInitialized = true;

			if (data.type == Reference.Type.Constant)
			{
				//
			}
			else
			{
				var variable = data.GetVariable();
				variable.OnModified.AddListener(OnDataModified);
			}

			OnDataModified(data.Value);

		}

		protected virtual void OnEnable()
		{
			if (isInitialized == false)
				return;


			if (data.type == Reference.Type.Constant)
			{
				//
			}
			else
			{
				var variable = data.GetVariable();
				variable.OnModified.AddListener(OnDataModified);
			}

			OnDataModified(data.Value);

		}

		protected virtual void OnDisable()
		{
			if (data.type == Reference.Type.Constant)
			{
				//
			}
			else
			{
				var variable = data.GetVariable();
				variable.OnModified.RemoveListener(OnDataModified);
			}

		}




		protected virtual void OnDataModified(T val)
		{
			string value;
			if (this.format == "")
			{
				value = val.ToString();
			}
			else
			{
				//var format = System.Text.RegularExpressions.Regex.Unescape(this.format.Trim());

				value = string.Format(format, val);
			}


			uiElement.text = value;

		}



	}

}
