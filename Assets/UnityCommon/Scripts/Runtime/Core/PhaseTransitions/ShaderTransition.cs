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

	[CreateAssetMenu(menuName = "Phase Transitions/Custom Shader", fileName = "ShaderPhaseTransition")]
	public class ShaderTransition : GamePhaseTransition
	{

		public GameObject transitionPrefab = null;

		public FloatReference transitionDuration;

		public string shaderProperty = "_FillAmount";


		public override void InitiateTransition()
		{
			base.InitiateTransition();

			int hash = Shader.PropertyToID(shaderProperty);

			var transitionObj = GameObject.Instantiate(transitionPrefab);

			DontDestroyOnLoad(transitionObj);

			var img = transitionObj.GetComponentInChildren<Image>();

			var mat = Instantiate(img.material);
			img.material = mat;

			var inDuration = transitionDuration.Value * 0.55f;
			var outDuration = transitionDuration.Value * 0.45f;

			AnimationModule.Animate<float>(val => mat.SetFloat(hash, val))
				.For(inDuration)
				.From(0f)
				.To(1f)
				.With(Interpolator.Smooth())
				.OnCompleted(
				() =>
				{

					AnimationModule.Animate<float>(val => mat.SetFloat(hash, val))
					.For(outDuration)
					.From(1f)
					.To(0f)
					.With(Interpolator.Smooth())
					.OnCompleted(
					() =>
					{
						Destroy(transitionObj);
						Destroy(mat);
					})
					.Start();


					try
					{
						CompleteTransition();

						//GC.Collect();

					}
					catch (Exception ex)
					{
						Debug.LogError("Flow Fade Transition Exception -> " + ex.ToString());
					}

					/*Conditional.Wait(0.1f).Do(() =>
					{*/


					//});

				}
				).Start();

		} // InitiateTransition end




	} // class end


}
