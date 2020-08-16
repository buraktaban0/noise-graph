using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Procedural
{

	public abstract class BaseNoise : ScriptableObject
	{

		public abstract NativeArray<float> GetHeightmapNative(int width, int height, Allocator nativeAllocator);


	}

}
