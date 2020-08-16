using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon.UI
{

	[RequireComponent(typeof(UnityEngine.UI.Image))]
	public class ImageFillSetter : UIBinder<Image>
	{
		private static readonly float threshold = 1e-3f;

		[SerializeField] private FloatReference maxValue = default;

		[SerializeField] private float sharpness = 30f;

		private float currentVal;

		private float targetVal = 0f;

		private void Awake()
		{
			targetVal = uiElement.fillAmount;
			currentVal = targetVal;
		}


		public void SetValue(float val)
		{
			targetVal = Mathf.Clamp01(val / maxValue.Value);

			this.enabled = true;
		}


		public void SetMaxValue(float val)
		{
			maxValue.Value = val;

			SetValue(val);

		}



		private void Update()
		{

			currentVal = Mathf.Lerp(currentVal, targetVal, Time.deltaTime * sharpness);

			uiElement.fillAmount = currentVal;

			if (Mathf.Abs(currentVal - targetVal) < threshold)
			{
				this.enabled = false;
			}

		}


	}

}
