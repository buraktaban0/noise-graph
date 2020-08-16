using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon.Runtime.Utility
{

	[RequireComponent(typeof(Graphic))]
	public class GraphicShaderVariableAnimator : MonoBehaviour
	{

		[SerializeField] private Graphic graphic = null;


		[SerializeField] private string propertyName = null;

		[SerializeField] private AnimationCurve curve = null;

		[SerializeField] private float initialValue = 0f;

		[SerializeField] private float duration = 1f;

		private int hash;


		private Material mat;

		private void OnValidate()
		{
			graphic = GetComponent<Graphic>();
		}


		private void Awake()
		{
			hash = Shader.PropertyToID(propertyName);

			mat = Instantiate(graphic.material);
			graphic.material = mat;

			mat.SetFloat(hash, initialValue);

		}



		public void FadeIn()
		{
			Animation<float> anim = new Animation<float>(val => mat.SetFloat(hash, val))
				.From(0f).To(1f).For(duration)
				.With(new Interpolator(curve))
				.Start();
		}


		public void FadeOut()
		{
			Animation<float> anim = new Animation<float>(val => mat.SetFloat(hash, val))
				.From(1f).To(0f).For(duration)
				.With(new Interpolator(curve))
				.Start();
		}



		private void OnDestroy()
		{
			Destroy(mat);
			mat = null;
		}


	}

}
