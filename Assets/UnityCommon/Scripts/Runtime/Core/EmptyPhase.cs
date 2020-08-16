using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Core;
using UnityEngine;

namespace UnityCommon.Runtime.Core
{
	[CreateAssetMenu(menuName = "Game Phases/Empty", fileName = "EmptyPhase")]
	public class EmptyPhase : GamePhase
	{

		public override void OnFixedUpdate()
		{

		}

		public override void OnUpdate()
		{

		}

		public override void OnLateUpdate()
		{

		}

		public override void OnApplicationQuit()
		{

		}


	}

}
