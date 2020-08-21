using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Procedural.Editor
{

	[CustomEditor(typeof(TreeNoise))]
	public class TreeNoiseEditor : UnityEditor.Editor
	{

		private TreeNoise tn;

		private ReorderableList ol;

		private bool elementModified = false;

		private void OnEnable()
		{
			tn = (TreeNoise)target;

			ol = new ReorderableList(tn.branches, typeof(NoiseLayer)
				, true, true, true, true);

			ol.drawHeaderCallback += DrawHeader;
			ol.drawElementCallback += DrawElement;
			//ol.onAddCallback += AddItem;
			//ol.onRemoveCallback += RemoveItem;

			ol.elementHeightCallback += ElementHeight;

			ol.showDefaultBackground = false;

			Undo.undoRedoPerformed += UndoRedoPerformed;
		}


		private void OnDisable()
		{
			ol.drawHeaderCallback -= DrawHeader;
			ol.drawElementCallback -= DrawElement;
			//ol.onAddCallback -= AddItem;
			//ol.onRemoveCallback -= RemoveItem;

			ol.elementHeightCallback -= ElementHeight;

			Undo.undoRedoPerformed -= UndoRedoPerformed;

		}

		private void UndoRedoPerformed()
		{
			//	tn.isModified = true;
		}


		private void DrawHeader(Rect rect)
		{
			GUI.Label(rect, "Layers");
		}

		private float ElementHeight(int index)
		{

			return 64;

		}


		private void DrawElement(Rect rect, int index, bool active, bool focused)
		{
			var branch = tn.branches[index];

			EditorGUI.BeginChangeCheck();

			var prop = serializedObject.FindProperty("branches").GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(rect, prop, false);



			if (prop.objectReferenceValue != null)
			{
				var editor = CreateEditor(prop.objectReferenceValue);
				editor.OnInspectorGUI();
			}
			


			

			serializedObject.ApplyModifiedProperties();

			//EditorGUI.PropertyField(rect, prop);


			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);


				//tn.isModified = true;


				elementModified = true;
			}

		}


		private void AddItem(ReorderableList list)
		{

			tn.branches.Add(null);

			EditorUtility.SetDirty(target);

			//tn.isModified = true;
		}

		private void RemoveItem(ReorderableList list)
		{

			tn.branches.RemoveAt(list.index);

			EditorUtility.SetDirty(target);

			//tn.isModified = true;
		}


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			ol.DoLayoutList();

		}

	}
}
