﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    public class AddLogicNode : LogicNode
    {
        public event Action<float> Vector1Output;

        [SerializeField]
        private float _valueA;
        
        [SerializeField]
        private float _valueB;
        
        public void Vector1AInput(float value)
        {
            Debug.Log("Vector1LogicNode SetValue " + value);
            _valueA = value;
            if (Vector1Output != null) Vector1Output(_valueA);
        }
        
        public void Vector1BInput(float value)
        {
            Debug.Log("Vector1LogicNode SetValue " + value);
            _valueA = value;
            if (Vector1Output != null) Vector1Output(_valueA);
        }
    }
}