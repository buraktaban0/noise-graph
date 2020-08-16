using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Procedural.Graph.Editor
{

	[ExecuteInEditMode]
	public class HiddenNoiseGraphBehaviour : MonoBehaviour
	{

		private List<Func<bool>> conditions = new List<Func<bool>>();
		private List<Action> responses = new List<Action>();

		public void RegisterCallback(Func<bool> condition, Action response)
		{
			conditions.Add(condition);
			responses.Add(response);
		}

		private void Update()
		{

			for (int i = conditions.Count - 1; i >= 0; i--)
			{
				if (conditions[i].Invoke())
				{
					responses[i].Invoke();
					conditions.RemoveAt(i);
					responses.RemoveAt(i);
				}
			}

		}

	}
}
