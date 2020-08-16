using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon.Runtime.Utility
{

	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemPauser : MonoBehaviour
	{

		[SerializeField] private ParticleSystem ps;

		private void OnValidate()
		{
			ps = GetComponent<ParticleSystem>();
		}

		public void Play()
		{
			ps.Play();
			var emission = ps.emission;
			emission.enabled = true;
		}

		public void Pause()
		{
			ps.Play();
			var emission = ps.emission;
			emission.enabled = false;
		}

	}
}
