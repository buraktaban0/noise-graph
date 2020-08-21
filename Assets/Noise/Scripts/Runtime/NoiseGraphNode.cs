using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using UnityEngine.UIElements;

namespace Noise.Runtime
{

	[System.Serializable]
	public abstract partial class NoiseGraphNode : IBinding
	{

		public string GUID = Guid.NewGuid().ToString();

		public virtual void PreUpdate()
		{

		}

		public virtual void Release()
		{

		}

		public virtual void Update()
		{

		}


		public virtual string GetNodeName()
		{
			return this.GetType().Name.Replace("Node", "");
		}


	}

}
