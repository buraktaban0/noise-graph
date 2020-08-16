using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UnityCommon.Events
{

	public class GameEventListener : MonoBehaviour
	{

		public string listenerName = "Unnamed Listener";


		public GameEvent gameEvent;

		public GameEvent[] gameEvents;

		public UnityEvent response;

		public void OnEnable()
		{
			if (Application.isPlaying)
			{
				gameEvent?.AddListener(OnEventFired);

				foreach (var ev in gameEvents)
				{
					ev.AddListener(OnEventFired);
				}

			}

		}

		public void OnDisable()
		{
			gameEvent?.RemoveListener(OnEventFired);

			foreach (var ev in gameEvents)
			{
				ev.RemoveListener(OnEventFired);
			}

		}

		public void OnEventFired(object args)
		{
			response?.Invoke();
		}

		public string GetName() => listenerName;
	}



}
