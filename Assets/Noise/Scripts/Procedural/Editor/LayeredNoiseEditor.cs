using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Procedural.Editor
{

	//[CustomEditor(typeof(LayeredNoise))]
	public class LayeredNoiseEditor : UnityEditor.Editor
	{

		private LayeredNoise ln;

		private ReorderableList ol;

		private bool elementModified = false;

		private void OnEnable()
		{
			ln = (LayeredNoise)target;

			ol = new ReorderableList(ln.layers, typeof(NoiseLayer)
				, true, true, true, true);

			ol.drawHeaderCallback += DrawHeader;
			ol.drawElementCallback += DrawElement;
			ol.onAddCallback += AddItem;
			ol.onRemoveCallback += RemoveItem;

			ol.elementHeightCallback += ElementHeight;

			Undo.undoRedoPerformed += UndoRedoPerformed;
		}


		private void OnDisable()
		{
			ol.drawHeaderCallback -= DrawHeader;
			ol.drawElementCallback -= DrawElement;
			ol.onAddCallback -= AddItem;
			ol.onRemoveCallback -= RemoveItem;

			ol.elementHeightCallback -= ElementHeight;

			Undo.undoRedoPerformed -= UndoRedoPerformed;

		}

		private void UndoRedoPerformed()
		{
			ln.isModified = true;
		}


		private void DrawHeader(Rect rect)
		{
			GUI.Label(rect, "Layers");
		}

		private float ElementHeight(int index)
		{

			return 156;

		}


		private void DrawElement(Rect rect, int index, bool active, bool focused)
		{
			var layer = ln.layers[index];

			EditorGUI.BeginChangeCheck();

			var prop = serializedObject.FindProperty("layers").GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(rect, prop);

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);


				ln.isModified = true;


				elementModified = true;
			}

		}


		private void AddItem(ReorderableList list)
		{

			serializedObject.FindProperty("layers").InsertArrayElementAtIndex(ln.layers.Count);
			serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(target);

			ln.isModified = true;
		}

		private void RemoveItem(ReorderableList list)
		{

			serializedObject.FindProperty("layers").DeleteArrayElementAtIndex(list.index);
			serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(target);

			ln.isModified = true;
		}


		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			base.OnInspectorGUI();

			GUILayout.Space(16);

			ol.DoLayoutList();

			if (EditorGUI.EndChangeCheck())
			{
				if (elementModified)
				{
					elementModified = false;
					return;
				}

				EditorUtility.SetDirty(target);

				ln.isModified = true;
			}

		}


	}

}
