using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Noise.Editor.Util
{
	public static class PortTypeUtil
	{
		public static bool HasConversionOperator(Type from, Type to)
		{
			TypeConverter conv = TypeDescriptor.GetConverter(from);
			return conv.CanConvertTo(to);
		}

		public static bool IsCompatible(Port connectedPort, FieldInfo fieldInfo)
		{
			PortData portData = (PortData) connectedPort.userData;

			IEnumerable<Type> types = new Type[] {fieldInfo.FieldType};
			if (fieldInfo.IsDefined(typeof(InputAttribute)))
			{
				types = types.Concat(fieldInfo.GetCustomAttribute<InputAttribute>().AdditionalAllowedTypes);
			}

			return portData.types.Intersect(types).Any();

			if (connectedPort.direction == Direction.Input)
			{
				// input
				return portData.types.Contains(fieldInfo.FieldType);
			}
			else
			{
				// output
			}
		}
	}
}
