using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor.Elements
{
	public class NodeIMGUIContainer : IMGUIContainer
	{

		public FieldInfo f { get; private set; }
		public SerializedObject serializedObject { get; private set; }
		public object nonSerializedObject { get; private set; }

		private Undo.UndoRedoCallback undoRedoPerformedCallback;

		~NodeIMGUIContainer()
		{
			Undo.undoRedoPerformed -= undoRedoPerformedCallback;
		}

		public NodeIMGUIContainer(FieldInfo f, SerializedObject serializedObject, SerializedFieldWrapper wrapper, object nonSerializedObject, Action onModified, string label = null)
		{

			var prop = serializedObject.FindProperty($"node.{f.Name}");

			Action<object> onImguiSetValue = (value) =>
			{
				f.SetValue(nonSerializedObject, value);
				Debug.Log(f.Name + " : " + value.ToString());
				onModified();
			};

			Action onImguiChangeInput = () =>
			{
				var value = f.GetValue(wrapper.node);
				onImguiSetValue(value);
			};

			Action onGUIHandler = () =>
			{

				if (this.visible == false)
					return;

				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 64;

				var wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true;

				var fieldWidth = EditorGUIUtility.fieldWidth;
				//EditorGUIUtility.fieldWidth = 24;

				EditorGUI.BeginChangeCheck();
				GUIContent labelContent = label == null ? new GUIContent(f.Name) : new GUIContent(label);
				GUILayout.FlexibleSpace();
				EditorGUILayout.PropertyField(prop, label: labelContent, true, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
				GUILayout.FlexibleSpace();
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(serializedObject.targetObject);
					onImguiChangeInput.Invoke();
				}
				else
				{
					var valueSerialized = f.GetValue(wrapper.node);
					var value = f.GetValue(nonSerializedObject);
					if (value.Equals(valueSerialized) == false)
					{
						//Debug.Log("Undo/Redo? " + value + " -> " + valueSerialized);
						EditorUtility.SetDirty(serializedObject.targetObject);
						onImguiSetValue(valueSerialized);
					}
				}

				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUIUtility.wideMode = wideMode;
				EditorGUIUtility.fieldWidth = fieldWidth;

			};


			undoRedoPerformedCallback = () =>
			{
				this.MarkDirtyRepaint();
			};
			Undo.undoRedoPerformed += undoRedoPerformedCallback;

			this.onGUIHandler = onGUIHandler;
		}

	}
}
