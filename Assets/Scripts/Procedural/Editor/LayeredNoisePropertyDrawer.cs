using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Procedural.Editor
{
	//[CustomPropertyDrawer(typeof(LayeredNoise))]
	public class LayeredNoisePropertyDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			EditorGUI.PropertyField(position, property, true);

			if (property.objectReferenceValue != null)
			{
				var editor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

				editor.OnInspectorGUI();
			}

		}

	}

}
