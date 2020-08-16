
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.RuntimeCollections;
using UnityEditor;
using UnityEngine;

namespace UnityCommon.ResourceManagement.Editor
{
	public static class EditorResourceManager
	{

		[MenuItem("Resources/Cache Resource Paths")]
		public static void CacheResourcePaths()
		{

			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
				AssetDatabase.SaveAssets();
			}


			if (!AssetDatabase.IsValidFolder("Assets/Resources/Preload"))
			{
				AssetDatabase.CreateFolder("Assets/Resources", "Preload");
				AssetDatabase.SaveAssets();
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources/Modules"))
			{
				AssetDatabase.CreateFolder("Assets/Resources", "Modules");
				AssetDatabase.SaveAssets();
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources/Variables"))
			{
				AssetDatabase.CreateFolder("Assets/Resources", "Variables");
				AssetDatabase.SaveAssets();
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources/GamePhases"))
			{
				AssetDatabase.CreateFolder("Assets/Resources", "GamePhases");
				AssetDatabase.SaveAssets();
			}


			var allResources = Resources.LoadAll("Preload");

			var paths = Resources.Load<StringCollection>("PreloadCache");

			if (paths == null)
			{
				paths = ScriptableObject.CreateInstance<StringCollection>();
				AssetDatabase.CreateAsset(paths, "Assets/Resources/PreloadCache.asset");
				AssetDatabase.SaveAssets();
			}
			else
			{
				paths.items = new List<string>();
			}


			foreach (var res in allResources)
			{
				string path = AssetDatabase.GetAssetPath(res);
				string resPath = path.Replace("\\", "/").Replace("Assets/Resources/Preload/", "");

				paths.items.Add(Path.ChangeExtension(resPath, null));
			}

			EditorUtility.SetDirty(paths);

			AssetDatabase.SaveAssets();

			paths = null;

			Resources.UnloadUnusedAssets();

		}



	}
}
