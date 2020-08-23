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
		public Type[] AdditionalAllowedTypes { get; private set; } = new Type[] { };

		public bool HasPort = true;

		public bool HasInlineEditor = true;

		public InputAttribute(params Type[] additionalAllowedTypes)
		{
			this.AdditionalAllowedTypes = additionalAllowedTypes;
		}




	}

}
