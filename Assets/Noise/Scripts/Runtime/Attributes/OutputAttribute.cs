using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noise.Runtime.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class OutputAttribute : Attribute
	{

		public OutputAttribute()
		{


		}


	}

}
