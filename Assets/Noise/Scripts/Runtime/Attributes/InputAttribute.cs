using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noise.Runtime.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class InputAttribute : Attribute
	{

		public bool IsConstant { get; private set; }

		public InputAttribute(bool isConstant = false)
		{
			IsConstant = isConstant;
		}


	}

}
