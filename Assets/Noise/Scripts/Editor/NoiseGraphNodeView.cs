using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Noise.Runtime;
using Noise.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using Noise.Editor.Util;
using Noise.Runtime.Nodes;
using System.Linq.Expressions;
using Noise.Editor.Elements;

namespace Noise.Editor
{

	public class NoiseGraphNodeView : Node
	{

		public string GUID => node.GUID;

		public VisualElement inputPortsContainer => inputContainer[0];
		public VisualElement inputFieldsContainer => inputContainer[1];

		public VisualElement outputPortsContainer => outputContainer[1];
		public VisualElement outputFieldsContainer => outputContainer[0];


		private static readonly Type InputAttrType = typeof(InputAttribute);
		private static readonly Type OutputAttrType = typeof(OutputAttribute);

		private NoiseGraphNode node;

		private SerializedFieldWrapper serializedNodeWrapper;
		private SerializedObject serializedObject;


		public NoiseGraphNodeView(NoiseGraphNode node)
		{
			this.node = node;
			this.title = node.GetNodeName();

			serializedNodeWrapper = ScriptableObject.CreateInstance<SerializedFieldWrapper>();

			serializedNodeWrapper.node = this.node;
			serializedObject = new SerializedObject(serializedNodeWrapper);

			serializedObject.Update();

			var ss = Resources.Load<StyleSheet>("NoiseGraph");

			styleSheets.Add(ss);
			inputContainer.styleSheets.Add(ss);
			outputContainer.styleSheets.Add(ss);

			inputContainer.AddToClassList("input");
			outputContainer.AddToClassList("output");


			inputContainer.Add(new VisualElement() { name = "PortsContainer" });
			inputContainer.Add(new VisualElement() { name = "BoundFieldsContainer" });
			outputContainer.Add(new VisualElement() { name = "BoundFieldsContainer" });
			outputContainer.Add(new VisualElement() { name = "PortsContainer" });

			//inputContainer.GetClasses().ToList().ForEach(s => Debug.Log(s));
			//this.AddToClassList("nodeContainer_c");

			BuildLayout();
		}


		public void BuildLayout()
		{
			var nodeType = node.GetType();

			var publicFields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);

			var inputFields = publicFields.Where(f => f.IsDefined(InputAttrType, true)).ToList();
			var outputFields = publicFields.Where(f => f.IsDefined(OutputAttrType, true)).ToList();

			var constInputFields = inputFields.Where(f => f.GetCustomAttribute<InputAttribute>().IsConstant).ToList();
			var variableInputFields = inputFields.Except(constInputFields).ToList();


			foreach (var f in constInputFields)
			{

				var boundElement = new NodeIMGUIContainer(f, serializedObject, serializedNodeWrapper, this.node, OnNodeModified);

				/*var boundElement = new PropertyField(prop, f.Name);
				boundElement.Bind(serializedObject);
				*/

				//var boundElement = NoiseGraphBindableFieldUtility.Get(f, (object val) => OnConstantFieldModified(f, val));

				//bindableField.Bind(serializedObject);


				//var bindableField = NoiseGraphBindableFieldUtility.Get(this.node, f);

				/*var prop = serializedObject.FindProperty($"node.{f.Name}");
				var bindableField = new PropertyField(prop, f.Name);
				bindableField.Bind(serializedObject);

				var modifiedEventWrapperType = typeof(NodeModifiedEventWrapper<>).MakeGenericType(f.FieldType);
				var modifiedEventWrapper = Activator.CreateInstance(modifiedEventWrapperType) as NodeModifiedEventWrapperBase;
				modifiedEventWrapper.node = this.node;
				modifiedEventWrapper.fieldInfo = f;
				var eventMethod = modifiedEventWrapperType.GetMethod("OnModified");
				var delegateMethod = eventMethod.CreateDelegate(Expression.GetDelegateType(
			(from parameter in eventMethod.GetParameters() select parameter.ParameterType)
			.Concat(new[] { eventMethod.ReturnType })
			.ToArray()), modifiedEventWrapper);

				var changeEventType = typeof(ChangeEvent<>).MakeGenericType(f.FieldType);
				var eventCallbackType = typeof(EventCallback<>).MakeGenericType(changeEventType);
				var regCllbMethod = bindableField.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Where(m => m.Name == "RegisterCallback").First();

				regCllbMethod.MakeGenericMethod(changeEventType);
				regCllbMethod.Invoke(bindableField, new object[]
				{
					modifiedEventWrapper.Event,
					TrickleDown.NoTrickleDown
				});

				*/

				/*
				bindableField.RegisterCallback<ChangeEvent<object>>(obj =>
				{
					Debug.Log("asd");
					f.SetValue(this.node, obj);
					OnNodeModified();
				});
				*/

				inputPortsContainer.Add(new Label(""));
				inputFieldsContainer.Add(boundElement);
			}

			foreach (var f in variableInputFields)
			{
				(var port, var el) = GetPortAndBoundElement(f, Direction.Input);
				inputPortsContainer.Add(port);
				inputFieldsContainer.Add(el);
			}

			foreach (var f in outputFields)
			{
				(var port, var el) = GetPortAndBoundElement(f, Direction.Output);
				outputPortsContainer.Add(port);
				outputFieldsContainer.Add(el);
			}

			RefreshExpandedState();
			RefreshPorts();


		}

		private (Port, VisualElement) GetPortAndBoundElement(FieldInfo f, Direction direction)
		{
			var port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Single, f.FieldType);
			port.name = f.Name;
			port.portName = f.Name;

			port.Q<Label>().RemoveFromHierarchy();

			var boundElement = new NodeIMGUIContainer(f, serializedObject, serializedNodeWrapper, this.node, OnNodeModified);

			return (port, boundElement);
		}


		public static NoiseGraphNodeView Build<T>() where T : NoiseGraphNode, new()
		{
			var node = new T();

			var view = new NoiseGraphNodeView(node);

			return view;
		}

		private void OnNodeModified()
		{
			Debug.Log("Node scope callback");
		}

	}

	public abstract class NodeModifiedEventWrapperBase
	{
		public FieldInfo fieldInfo;
		public NoiseGraphNode node;

	}


	public class NodeModifiedEventWrapper<T> : NodeModifiedEventWrapperBase
	{

		public void OnModified(ChangeEvent<T> ev)
		{
			Debug.Log("WIN!");
		}

	}



}
