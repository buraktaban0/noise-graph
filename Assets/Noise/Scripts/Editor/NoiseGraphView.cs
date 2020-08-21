using System.Collections;
using System.Collections.Generic;
using Noise.Runtime.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noise.Editor
{

	public class NoiseGraphView : GraphView
	{


		public NoiseGraphView()
		{
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			styleSheets.Add(Resources.Load<StyleSheet>("NoiseGraph"));

			var grid = new GridBackground();
			Insert(0, grid);
			grid.StretchToParentSize();

			var nodeView = NoiseGraphNodeView.Build<TestNode>();

			this.AddElement(nodeView);
			

		}


	}

}
