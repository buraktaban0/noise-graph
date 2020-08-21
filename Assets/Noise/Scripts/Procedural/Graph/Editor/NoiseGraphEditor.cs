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

	[CustomNodeGraphEditor(typeof(NoiseGraph))]
	public class NoiseGraphEditor : NodeGraphEditor
	{


		public override void OnOpen()
		{
			base.OnOpen();
			
			window.onClose += new NodeEditorWindow.WindowStateEvent(OnClose);

			window.titleContent = new GUIContent("Noise Graph");

			EditorApplication.update += ((NoiseGraph)target).OnUpdate;
		}

		public void OnClose()
		{
			base.OnDestroy();

			window.onClose -= OnClose;

			EditorApplication.update -= ((NoiseGraph)target).OnUpdate;
		}

	}

}
