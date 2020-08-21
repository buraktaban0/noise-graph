using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace Noise.Editor.Util
{

	public static class NoiseGraphBindableFieldUtility
	{

		private static readonly Dictionary<Type, Func<VisualElement>> providers;

		static NoiseGraphBindableFieldUtility()
		{
			providers = new Dictionary<Type, Func<VisualElement>>()
			{
				{ typeof(float), () =>
					{
						return new FloatField();
					}
				}
			};
		}


		private static VisualElement GetVisualElement(FieldInfo f, Action<object> callback)
		{

			VisualElement el = null;
			if (f.FieldType == typeof(float))
			{
				el = new FloatField(f.Name);
				el.RegisterCallback<ChangeEvent<float>>((ev) =>
				{
					callback.Invoke(ev.newValue);
				});
			}
			else if (f.FieldType == typeof(int))
			{
				el = new IntegerField(f.Name);
				el.RegisterCallback<ChangeEvent<int>>((ev) =>
				{
					callback.Invoke(ev.newValue);
				});
			}



			return el;

		}

		public static VisualElement Get(FieldInfo f, Action<object> callback)
		{

			var element = GetVisualElement(f, callback);

			return element;

		}

	}

}
