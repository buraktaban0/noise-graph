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

		private static readonly float defaultFieldWidthOverride = 38;
		private static readonly bool defaultWideModeOverride = false;
		private struct IMGUITypeOverrides
		{
			public float fieldWidthOverride;
			public bool wideModeOverride;

			public IMGUITypeOverrides(bool dummy = true)
			{
				fieldWidthOverride = defaultFieldWidthOverride;
				wideModeOverride = defaultWideModeOverride;
			}
		}

		private static readonly Dictionary<Type, IMGUITypeOverrides> imguiTypeOverrides = new Dictionary<Type, IMGUITypeOverrides>()
		{
			{ typeof(Vector2), new IMGUITypeOverrides{fieldWidthOverride = defaultFieldWidthOverride * 3f, wideModeOverride = true}} ,
			{ typeof(Vector3), new IMGUITypeOverrides{fieldWidthOverride = defaultFieldWidthOverride * 4f, wideModeOverride = true}} ,
			{ typeof(Vector4), new IMGUITypeOverrides{fieldWidthOverride = defaultFieldWidthOverride, wideModeOverride = true}} ,
		};

		public FieldInfo f { get; private set; }
		public SerializedObject serializedObject { get; private set; }
		public object nonSerializedObject { get; private set; }

		public bool labelOnly { get; set; } = false;

		private Undo.UndoRedoCallback undoRedoPerformedCallback;


		~NodeIMGUIContainer()
		{
			Undo.undoRedoPerformed -= undoRedoPerformedCallback;
		}

		public NodeIMGUIContainer(FieldInfo f, SerializedObject serializedObject, SerializedNodeWrapper wrapper, object nonSerializedObject, Action onModified, string label = null)
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

			var typeOverride = new IMGUITypeOverrides();

			if (imguiTypeOverrides.ContainsKey(f.FieldType))
			{
				typeOverride = imguiTypeOverrides[f.FieldType];
			}

			Action onGUIHandler = () =>
			{

				if (this.visible == false)
					return;

				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 64;

				var wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = typeOverride.wideModeOverride;

				var fieldWidth = EditorGUIUtility.fieldWidth;
				EditorGUIUtility.fieldWidth = typeOverride.fieldWidthOverride;

				serializedObject.Update();
				
				GUIContent labelContent = label == null ? new GUIContent(f.Name) : new GUIContent(label);

				GUILayout.BeginVertical();

				EditorGUI.BeginChangeCheck();
				GUILayout.FlexibleSpace();
				if (labelOnly)
				{
					EditorGUILayout.LabelField(labelContent);
				}
				else
				{
					bool expanded = EditorGUILayout.PropertyField(prop, label: labelContent, true);
				}
				GUILayout.FlexibleSpace();
				serializedObject.ApplyModifiedProperties();
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

				GUILayout.EndVertical();


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
