using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Singletons;
using UnityEngine;

namespace UnityCommon.Runtime.Utility
{

	public class KeepAlive : SingletonBehaviour<KeepAlive>
	{

		private void Awake()
		{

			if (!SetupInstance())
			{
				return;
			}

		}

	}

}
