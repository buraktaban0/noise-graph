﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    public class LogicGraphObject : ScriptableObject
    {
        [SerializeField] private LogicGraphData _logicGraphData;

        public LogicGraphData GraphData
        {
            get { return _logicGraphData; }
        }

        public void Initialize(LogicGraphData logicGraphData)
        {
            _logicGraphData = logicGraphData;
        }

        /// <summary>
        /// Creates instances, and connects together the instances, of a LogicGraph.
        /// Will flush the previously saved instances in the process. TODO investigate if any previous instances can be recycled
        /// </summary>
        public void LoadLogicNodeGraph(List<LogicNode> nodes, List<LogicNode> inputNodes, List<LogicNode> outputNodes)
        {
            if (GraphData == null)
                return;

            nodes.Clear();
            for (int i = 0; i < GraphData.SerializedNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(GraphData.SerializedNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    nodes.Add(node);
                }
            }

            inputNodes.Clear();
            Debug.Log(GraphData.SerializedInputNodes.Count);
            for (int i = 0; i < GraphData.SerializedInputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(GraphData.SerializedInputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    inputNodes.Add(node);
                }
            }

            outputNodes.Clear();
            for (int i = 0; i < GraphData.SerializedOutputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(GraphData.SerializedOutputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    outputNodes.Add(node);
                }
            }

            foreach (var serializedEdge in GraphData.SerializedEdges)
            {
                LogicNode sourceNode = FindNodeByGuid(serializedEdge.SourceNodeGuid, nodes, inputNodes, outputNodes);
                if (sourceNode == null)
                {
                    Debug.LogWarning($"Source node is null for edge. SourceNodeGuid: {serializedEdge.SourceNodeGuid} SourceMemberName: {serializedEdge.SourceMemberName}");
                    continue;
                }

                LogicNode targetNode = FindNodeByGuid(serializedEdge.TargetNodeGuid, nodes, inputNodes, outputNodes);
                if (targetNode == null)
                {
                    Debug.LogWarning($"Target node is null for edge. TargetNodeGuid: {serializedEdge.TargetNodeGuid} TargetMemberName: {serializedEdge.TargetMemberName}");
                    continue;
                }

                MethodInfo targetMethodInfo = MethodInfoByName(targetNode, serializedEdge.TargetMemberName);
                if (targetMethodInfo == null)
                {
                    Debug.LogWarning($"Target method is null for edge. TargetNodeGuid: {serializedEdge.TargetNodeGuid} TargetMemberName: {serializedEdge.TargetMemberName}");
                    continue;
                }

                SubscribeToEventByName(sourceNode, serializedEdge.SourceMemberName, targetNode, targetMethodInfo);
            }
        }

        private MethodInfo MethodInfoByName(LogicNode node, string memberName)
        {
//            Debug.Log($"Finding method {memberName} on {node.GetType()}");
            var methods = node.GetType()
                .GetMethods(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
//                    Debug.Log(method.Name);
                if (method.Name == memberName)
                {
                    return method;
                }
            }

            return null;
        }

        private void SubscribeToEventByName(
            LogicNode sourceNode,
            string memberName,
            LogicNode targetNode,
            MethodInfo targetMethodInfo)
        {
            try
            {
                var events = sourceNode.GetType()
                    .GetEvents(BindingFlags.Public |
                               BindingFlags.NonPublic |
                               BindingFlags.Instance);
                foreach (EventInfo eventInfo in events)
                {
                    if (eventInfo.Name == memberName)
                    {
                        Type type = eventInfo.EventHandlerType;
                        Delegate handler = Delegate.CreateDelegate(type, targetNode, targetMethodInfo);
                        eventInfo.AddEventHandler(sourceNode, handler);
                        return;
                    }
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"Subscribing to event {memberName} on {sourceNode.GetType()} failed. {e.Message}");
            }
        }

        private LogicNode FindNodeByGuid(string guid, params List<LogicNode>[] lists)
        {
            foreach (var list in lists)
            {
                LogicNode node = list.Find(n => n.NodeGuid == guid);
                if (node != null) return node;
            }

            return null;
        }

        private LogicNode CreateLogicNodeFromSerializedNode(SerializedNode serializedNode)
        {
            Debug.Log("Created node " + serializedNode.NodeType);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(LogicNode)))
                    {
                        if (type.Name == serializedNode.NodeType)
                        {
                            return JsonUtility.FromJson(serializedNode.JSON, type) as LogicNode;
                        }
                    }
                }
            }

            Debug.LogError("Failed to create node " + serializedNode.NodeType);
            return null;
        }
    }
}