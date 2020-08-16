using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procedural.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Procedural.Native
{

	[System.Serializable]
	public struct NativeArrayPromise
	{
		public NativeArray<float> data;
		public int width;
		public int height;

		public int Length => data.Length;
		public bool IsCreated { get; private set; }

		public JobHandle dependencyHandle;

		/// <summary>
		/// Does not check if it's power of two or not, be careful!
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="allocator"></param>
		public NativeArrayPromise(int width, int height, Allocator allocator) : this()
		{
			this.width = width;
			this.height = height;
			data = new NativeArray<float>(width * height, allocator);

			dependencyHandle = default(JobHandle);

			IsCreated = true;
		}


		public void Dispose()
		{
			data.Dispose(dependencyHandle);

			IsCreated = false;
		}





		public static NativeArrayPromise operator +(NativeArrayPromise a, NativeArrayPromise b)
		{
			var job = new AddJob
			{
				a = a.data,
				b = b.data
			};

			var handle = job.Schedule(a.data.Length, 64, JobHandle.CombineDependencies(a.dependencyHandle, b.dependencyHandle));

			b.dependencyHandle = handle;

			return b;
		}

		public static NativeArrayPromise operator -(NativeArrayPromise a, NativeArrayPromise b)
		{
			var job = new SubtractJob
			{
				a = a.data,
				b = b.data
			};

			var handle = job.Schedule(a.data.Length, 64, JobHandle.CombineDependencies(a.dependencyHandle, b.dependencyHandle));

			b.dependencyHandle = handle;

			return b;
		}

		public static NativeArrayPromise operator *(NativeArrayPromise a, NativeArrayPromise b)
		{
			var job = new MultiplyJob
			{
				a = a.data,
				b = b.data
			};

			var handle = job.Schedule(a.data.Length, 64, JobHandle.CombineDependencies(a.dependencyHandle, b.dependencyHandle));

			b.dependencyHandle = handle;

			return b;
		}

	}
}
