using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Noise.Editor.Util
{

	public static class NoiseGraphAssetHandler
	{

		[OnOpenAsset(1)]
		public static bool OnAssetOpened(int instanceID, int line)
		{
			NoiseGraph graph = EditorUtility.InstanceIDToObject(instanceID) as NoiseGraph;

			if (graph == null)
				return false;

			NoiseGraphWindow.currentViewedGraph = graph;
			NoiseGraphWindow.Open();

			return true;
		}

	}

}
