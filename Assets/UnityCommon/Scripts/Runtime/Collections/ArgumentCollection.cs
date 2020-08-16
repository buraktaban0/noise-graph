using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.Collections
{

	public class ArgumentCollection
	{

		private List<string> args;

		public ArgumentCollection(string[] args)
		{
			this.args = new List<string>(args);
		}

		public ArgumentCollection(string argsString)
		{
			var argsArr = argsString.Split(' ');
			args = new List<string>(argsArr);
		}

		public ArgumentCollection()
		{
			args = new List<string>();
		}



		public void Add(string arg)
		{
			args.Add(arg);
		}

		public void Add(string key, string value)
		{
			args.Add(key);
			args.Add(value);
		}



		public bool HasArgument(string arg)
		{
			return args.Contains(arg);
		}

		public string GetValue(string key)
		{
			for (int i = 0; i < args.Count - 1; i++)
			{
				if (args[i] == key)
				{
					return args[i + 1];
				}
			}

			return null;
		}



		public override string ToString()
		{
			string argString = "";
			for (int i = 0; i < args.Count; i++)
			{
				if (i > 0)
				{
					argString += " ";
				}

				argString += args[i];
			}

			return argString;
		}

	}



	public interface IArgCollectionEntry
	{

		string WriteToArgs(ArgumentCollection coll);

		void ReadFromArgs(ArgumentCollection coll);

	}

}