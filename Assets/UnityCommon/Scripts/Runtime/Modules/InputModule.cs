using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Modules;
using UnityCommon.Utilities;
using UnityEngine;

namespace UnityCommon.Modules
{

	[CreateAssetMenu(menuName = "Modules/Input Module")]
	public class InputModule : Module<InputModule>
	{

		private static Vector3 lastPos;

		private static Vector3 currentPos;

		public static Vector3 MouseDelta => currentPos - lastPos;

		public static Vector3 MouseDeltaHeightNormalized => MouseDelta / Screen.height;
		public static Vector3 MouseDeltaWidthNormalized => MouseDelta / Screen.width;


		public override void OnStart()
		{

		}



		public override void OnUpdate()
		{


			if (InputUtility.GetMouseHold())
			{
				if (InputUtility.GetMouseDown())
				{
					lastPos = Input.mousePosition;
				}
				else
				{
					lastPos = currentPos;
				}

				if (InputUtility.GetMouseUp())
				{
					currentPos = lastPos;
				}
				else
				{
					currentPos = Input.mousePosition;
				}



			}
			else
			{
				lastPos = currentPos;
			}

		}



		public override void OnLateUpdate()
		{

		}



		public override void OnStop()
		{

		}


	}

}