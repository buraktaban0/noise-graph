using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.Events
{
	public interface IGameEventListener
	{

		void OnEventFired();

		string GetName();

	}
}
