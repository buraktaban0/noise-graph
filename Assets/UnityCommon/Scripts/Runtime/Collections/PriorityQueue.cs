using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommon.Collections
{

	public class PriorityQueue<T> where T : IComparable<T>
	{

		public int Count => items.Count;


		private List<T> items = new List<T>(64);


		public PriorityQueue()
		{
		}


		public void Enqueue(T item)
		{
			for (int i = 0; i < items.Count; i++)
			{
				var c = items[i].CompareTo(item);
				if (c > 0)
				{
					items.Insert(i, item);
					return;
				}

			}

			items.Add(item);

		}

		public T Dequeue()
		{
			var i = items.Count - 1;
			var item = items[i];
			items.RemoveAt(i);
			return item;
		}

		public T Peek()
		{
			return items[items.Count - 1];
		}

		public void Remove(T item)
		{
			items.Remove(item);
		}


		public void Clear()
		{
			items.Clear();
		}



	}

}
