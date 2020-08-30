using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Noise.Runtime.Serialization
{

	[System.Serializable]
	public class SerializedLink
	{

		public int guid0;
		public string field0;

		public int guid1;
		public string field1;

		public SerializedLink(int guid0, string field0, int guid1, string field1)
		{
			this.guid0 = guid0;
			this.field0 = field0;

			this.guid1 = guid1;
			this.field1 = field1;
		}
	}

}

