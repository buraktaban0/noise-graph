using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Core.PhaseTransitions;
using UnityCommon.Modules;
using UnityCommon.ResourceManagement;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCommonExample.Core.PhaseTransitions
{

	[CreateAssetMenu(menuName = "Phase Transitions/Color Fade", fileName = "ColorFadePhaseTransition")]
	public class ColorFadeTransition : GamePhaseTransition
	{

		public GameObject transitionPrefab = null;

		public FloatReference transitionDuration;


		public override void InitiateTransition()
		{
			base.InitiateTransition();

			var transitionObj = GameObject.Instantiate(transitionPrefab);

			DontDestroyOnLoad(transitionObj);

			//transitionObj.GetComponentInChildren<Image>().color = color.Value;

			var canvasGroup = transitionObj.GetComponent<CanvasGroup>();

			var inDuration = transitionDuration.Value * 0.6f;
			var outDuration = transitionDuration.Value * 0.4f;

			AnimationModule.Animate<float>(val => canvasGroup.alpha = val)
				.For(inDuration)
				.From(0f)
				.To(1f)
				.With(Interpolator.Smooth())
				.OnCompleted(
				() =>
				{

					AnimationModule.Animate<float>(val => canvasGroup.alpha = val)
					.For(outDuration)
					.From(1f)
					.To(0f)
					.With(Interpolator.Accelerate())
					.OnCompleted(
					() =>
					{
						Destroy(transitionObj);
					})
					.Start();


					try
					{
						CompleteTransition();

						//GC.Collect();

					}
					catch (Exception ex)
					{
						Debug.LogError("Color Fade Transition Exception -> " + ex.ToString());
					}

					/*Conditional.Wait(0.1f).Do(() =>
					{*/


					//});

				}
				).Start();

		} // InitiateTransition end




	} // class end


}
