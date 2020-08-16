using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Procedural.Editor
{

	[CustomPropertyDrawer(typeof(NoiseLayer))]
	public class NoiseLayerPropertyDrawer : PropertyDrawer
	{

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			var curveRect = new Rect(position.x, position.y, position.width, 20);
			var blendModeRect = new Rect(position.x, position.y + 22, position.width, 20);
			var amplitudeRect = new Rect(position.x, position.y + 44, position.width, 20);
			var frequencyRect = new Rect(position.x, position.y + 66, position.width, 20);
			var offsetRect = new Rect(position.x, position.y + 88, position.width, 20);
			var visibleRect = new Rect(position.x, position.y + 110, position.width, 20);

			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 128;

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(curveRect, property.FindPropertyRelative("curve"), GUIContent.none);
			EditorGUI.PropertyField(blendModeRect, property.FindPropertyRelative("blendMode"), new GUIContent("Blend mode"));
			EditorGUI.PropertyField(amplitudeRect, property.FindPropertyRelative("amplitude"), new GUIContent("Amplitude"));
			EditorGUI.PropertyField(frequencyRect, property.FindPropertyRelative("frequency"), new GUIContent("Frequency"));
			EditorGUI.PropertyField(offsetRect, property.FindPropertyRelative("offset"), new GUIContent("Offset"));
			EditorGUI.PropertyField(visibleRect, property.FindPropertyRelative("visible"), new GUIContent("Visible"));

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUIUtility.labelWidth = labelWidth;

			EditorGUI.EndProperty();
		}

	}

}
