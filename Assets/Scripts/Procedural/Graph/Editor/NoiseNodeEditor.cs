using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XNodeEditor;

namespace Procedural.Graph.Editor
{
	[CustomNodeEditor(typeof(XNode.Node))]
	public class NoiseNodeEditor : NodeEditor
	{

		public override void OnCreate()
		{
			base.OnCreate();

		}


		public override void OnBodyGUI()
		{
			EditorGUI.BeginChangeCheck();

			base.OnBodyGUI();

			if (EditorGUI.EndChangeCheck())
			{
				target.isDirty = true;
			}

		}

	}

}
