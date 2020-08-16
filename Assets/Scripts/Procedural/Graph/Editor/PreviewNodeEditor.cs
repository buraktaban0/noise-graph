using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Procedural.Graph.Editor
{

	[CustomNodeEditor(typeof(PreviewNode))]
	public class PreviewNodeEditor : NoiseNodeEditor
	{

		public PreviewNode node;

		private Material mat;

		public override void OnCreate()
		{
			base.OnCreate();

			mat = new Material(Shader.Find("Unlit/S_Preview"));

			node = (PreviewNode)target;

			
		}


		public override int GetWidth()
		{
			return (int)(base.GetWidth() * 1.25f);

		}


		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			var rect = EditorGUILayout.GetControlRect(false, 225f);
			//rect.x = node.previewTextureSize * 0.2f;
			//rect.width = node.previewTextureSize * 2f;

			//GUILayout.Box(node.previewTexture);

			/*rect.width = node.previewTextureSize;
			rect.height = node.previewTextureSize;*/


			EditorGUI.DrawPreviewTexture(rect, node.GetBuffer(), mat);

		}

	}
}
