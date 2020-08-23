using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime.Attributes;
using Procedural.GPU;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Runtime.Nodes
{

	[System.Serializable]
	public abstract partial class NoiseGraphNode
	{

		protected static readonly int HASH_TEXA = Shader.PropertyToID("_TexA");
		protected static readonly int HASH_TEXB = Shader.PropertyToID("_TexB");
		protected static readonly int HASH_TEXC = Shader.PropertyToID("_TexC");
		protected static readonly int HASH_VALUE = Shader.PropertyToID("_Value");
		protected static readonly int HASH_FACTOR = Shader.PropertyToID("_Factor");
		protected static readonly int HASH_RANGE = Shader.PropertyToID("_Range");
		protected static readonly int HASH_MAP = Shader.PropertyToID("_Map");
		protected static readonly int HASH_PERM = Shader.PropertyToID("p");
		protected static readonly int HASH_CURVEDATA = Shader.PropertyToID("_CurveData");
		protected static readonly int HASH_SAMPLECOUNT = Shader.PropertyToID("_SampleCount");


		private static readonly System.Random rand = new System.Random();

		public int GUID = rand.Next();

		public GPUNoiseBufferHandle buffer;

		public bool wasProcessed { get; protected set; } = false;

		private Dictionary<string, object> inputs = new Dictionary<string, object>();
		private Dictionary<string, object> outputs = new Dictionary<string, object>();

		public virtual bool ShouldCreateBuffer() => true;


		//protected void CacheFields()
		//{
		//	inputFields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
		//		.Where(f => f.IsDefined(typeof(InputAttribute)))
		//		.ToDictionary(f => f.Name);
		//}

		protected object GetInput(string name, object fallbackValue)
		{
			object value;
			if (inputs.TryGetValue(name, out value))
			{
				return value;
			}

			return fallbackValue;
		}

		public void SetInput(string name, object value)
		{
			inputs[name] = value;
		}

		public object GetOutput(string name, object fallbackValue)
		{
			object value;
			if (outputs.TryGetValue(name, out value))
			{
				return value;
			}

			return fallbackValue;
		}

		public void SetOutput(string name, object value)
		{
			outputs[name] = value;
		}

		public virtual void Process()
		{
			ValidateBuffer();

			wasProcessed = true;
		}

		public void ValidateBuffer()
		{
			if (ShouldCreateBuffer() == false)
			{
				return;
			}

			if (buffer.IsCreated == false)
			{
				buffer.Release();
				buffer = new GPUNoiseBufferHandle(512, 512);
			}
		}

		public virtual void Clear()
		{
			wasProcessed = false;
			inputs.Clear();
			outputs.Clear();

			buffer.Release();
		}

		public virtual string GetNodeName()
		{
			return this.GetType().Name.Replace("Node", "");
		}


	}

}
