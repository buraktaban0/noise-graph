using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Noise.Runtime.Util
{
	public static class PortTypeUtil
	{
		public static bool HasConversionOperator(Type from, Type to)
		{
			TypeConverter conv = TypeDescriptor.GetConverter(from);
			return conv.CanConvertTo(to);

		}
	}
}
