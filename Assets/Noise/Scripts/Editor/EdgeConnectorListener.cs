using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace Noise.Editor
{
	public class EdgeConnectorListener : IEdgeConnectorListener
	{
		private readonly NoiseGraphView _logicGraphEditorView;
		private readonly SearchWindowProvider _searchWindowProvider;

		public EdgeConnectorListener(NoiseGraphView logicGraphEditorView, SearchWindowProvider searchWindowProvider)
		{
			_logicGraphEditorView = logicGraphEditorView;
			_searchWindowProvider = searchWindowProvider;
		}

		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			Port draggedPort = null;
			if (edge.input != null)
			{
				// Looking for output
				draggedPort = edge.input;
				_searchWindowProvider.IsConnectedInput = true;
				Debug.Log("Input");
			}
			else
			{
				// Looking for input
				draggedPort = edge.output;
				_searchWindowProvider.IsConnectedInput = false;
				Debug.Log("Output");
			}


			_searchWindowProvider.ConnectedPort = draggedPort;
			SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
			                  _searchWindowProvider);
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			_logicGraphEditorView.AddEdge(edge);
		}
	}
}
