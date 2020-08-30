using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noise.Editor.Util;
using Noise.Runtime.Attributes;
using Noise.Runtime.Util;
using Procedural.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NoiseGraphNode = Noise.Runtime.Nodes.NoiseGraphNode;

namespace Noise.Editor
{
	public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
	{
		public Port ConnectedPort    { get; set; }
		public bool IsConnectedInput { get; set; }

		private NoiseGraphWindow window;
		private NoiseGraphView graphView;

		public static SearchWindowProvider Build(NoiseGraphWindow window, NoiseGraphView graphView)
		{
			var provider = ScriptableObject.CreateInstance<SearchWindowProvider>();
			provider.window = window;
			provider.graphView = graphView;
			return provider;
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var baseType = typeof(NoiseGraphNode);
			var attrType = typeof(NodeAttribute);
			var inAttrType = typeof(InputAttribute);
			var outAttrType = typeof(OutputAttribute);

			var targetPortTypeAttr = IsConnectedInput ? outAttrType : inAttrType;

			var entries = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
			};

			var allNodeTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t =>
			{
				return t.IsAbstract == false && t.IsSubclassOf(baseType) && t.IsDefined(attrType);
			}).ToList();

			entries.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 1));

			if (ConnectedPort == null)
			{
				var nodeTypeEntries = allNodeTypes.Select(t =>
				{
					var nodeTypeName = t.GetCustomAttribute<NodeAttribute>().Name;
					return new SearchTreeEntry(new GUIContent(nodeTypeName))
					{
						userData = t,
						level = 2
					};
				});
				entries.AddRange(nodeTypeEntries);
				return entries;
			}

			var fieldData = allNodeTypes.SelectMany(t =>
			{
				return t.GetFields(BindingFlags.Public | BindingFlags.Instance)
				        .Where(f => f.IsDefined(targetPortTypeAttr) && PortTypeUtil.IsCompatible(ConnectedPort, f));
			}).ToList();

			Debug.Log(fieldData.Count);

			var fieldEntries = fieldData.Select(f =>
			{
				var nodeTypeName = $"{f.DeclaringType.GetCustomAttribute<NodeAttribute>().Name}: {f.Name}";
				return new SearchTreeEntry(new GUIContent(nodeTypeName))
				{
					userData = f,
					level = 2
				};
			});

			entries.AddRange(fieldEntries);

			return entries;
		}

		public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
		{
			var wPos = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
			                                                        context.screenMousePosition -
			                                                        window.position.position);
			var lPos = graphView.contentViewContainer.WorldToLocal(wPos);

			if (ConnectedPort == null)
			{
				var type = searchTreeEntry.userData as Type;

				if (type == null)
					return false;

				graphView.AddNode(type, lPos);
				return true;
			}

			if (ConnectedPort != null)
			{
				// Auto connect, dragged from existing port
				Debug.Log("Auto connect");

				var field = searchTreeEntry.userData as FieldInfo;

				if (field == null)
					return false;

				var type = field.DeclaringType;

				var view = graphView.AddNode(type, lPos);

				VisualElement container = null;
				if (IsConnectedInput)
				{
					container = view.outputContainer;
				}
				else
				{
					container = view.inputContainer;
				}

				var port = container.Q<Port>(name: field.Name);

				graphView.ConnectPorts(ConnectedPort, port);
				
			}

			return true;
		}
	}
}
