using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor
{
	public class GenericBindingWrapperEvent : EventBase<GenericBindingWrapperEvent>, IChangeEvent
	{

		public GenericBindingWrapperEvent()
		{ }

		public float previousValue { get; protected set; }
		public float newValue { get; protected set; }

		protected override void Init()
		{
			base.Init();
		}

		public static GenericBindingWrapperEvent GetPooled(float previousValue, float newValue)
		{
			return new GenericBindingWrapperEvent { previousValue = previousValue, newValue = newValue};
		}

	}
}
