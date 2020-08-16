using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.Runtime.Collections
{

	public class FixedSizeQueue<T>
	{

		private List<T> list;

		public int Size { get; private set; }

		public FixedSizeQueue(int size)
		{
			Size = size;
			list = new List<T>(size);
		}


		public ReadOnlyCollection<T> AsList()
		{
			return list.AsReadOnly();
		}


		public void Enqueue(T item)
		{
			if (list.Count >= Size)
			{
				list.RemoveAt(0);
			}


			list.Add(item);

		}


		public T Dequeue()
		{
			if (list.Count < 1)
				return default;

			var item = list[0];

			list.RemoveAt(0);

			return item;
		}


	}

}
