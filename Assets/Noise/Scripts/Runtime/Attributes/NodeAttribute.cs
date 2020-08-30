using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noise.Runtime.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class NodeAttribute : Attribute
	{
		public string Name = "Unnamed Node";
	}
}
