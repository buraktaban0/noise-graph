using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommon.Variables;
using UnityEngine;

namespace Assets.UnityCommon.Scripts.Tests
{
	public class VarTest2 : MonoBehaviour
	{

		private FloatVariable myFloatVarxXXX1;

		private int myInt;


		private void Awake()
		{
			myFloatVarxXXX1 = Variable.Create<FloatVariable>();
		}

		private void Update()
		{
			myFloatVarxXXX1.Value += Time.deltaTime * 2f;

		}


	}
}
