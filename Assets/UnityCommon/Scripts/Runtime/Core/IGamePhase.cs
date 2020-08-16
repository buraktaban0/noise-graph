using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.Core
{
	public interface IGamePhase
	{
		bool IsRunning { get; set; }

		void OnStart();

		void OnStop();

		void OnFixedUpdate();

		void OnUpdate();

		void OnLateUpdate();


		void OnApplicationQuit();

	}
}
