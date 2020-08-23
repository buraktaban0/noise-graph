using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Noise;
using Noise.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor
{

	public class NoiseGraphWindow : EditorWindow
	{

		public static NoiseGraph currentViewedGraph { get; set; }

		private NoiseGraphView graphView;

		private void OnEnable()
		{
			GenerateGraphView();

			GenerateToolbar();

			if (currentViewedGraph != null)
			{
				DeserializeBuildCurrentGraph();
			}

		}

		private void OnDisable()
		{
			if (graphView != null)
				rootVisualElement.Remove(graphView);
		}


		private void GenerateToolbar()
		{
			var toolbar = new Toolbar();

			/*
			var fileNameTextField = new TextField("File Name:");
			fileNameTextField.SetValueWithoutNotify(fileName);
			fileNameTextField.MarkDirtyRepaint();
			fileNameTextField.RegisterValueChangedCallback(e => fileName = e.newValue);

			toolbar.Add(fileNameTextField);
			toolbar.Add(new Button(SaveGraph) { text = "Save" });
			toolbar.Add(new Button(CreateGraph) { text = "Create New" });
			*/

			rootVisualElement.Add(toolbar);
		}

		private void GenerateGraphView()
		{

			graphView = new NoiseGraphView
			{
				name = "Noise Graph View"
			};

			graphView.StretchToParentSize();

			rootVisualElement.Add(graphView);

		}

		private void DeserializeBuildCurrentGraph()
		{
			var nodes = currentViewedGraph.DeserializeNodes();

			var nodeViews = nodes.Select(node => new NoiseGraphNodeView(node, graphView)).ToList();

		}


		[MenuItem("Noise Graph/Open")]
		public static void Open()
		{
			var window = GetWindow<NoiseGraphWindow>();
			window.titleContent = new GUIContent("Noise Graph");
		}

	}


}