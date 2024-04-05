using System;
using UnityEngine;

namespace NetworkAI
{
	[Serializable]
	public class StateTransition : MonoBehaviour
	{
		public StateCondition Condition; // 다른 상태로 전환되는 조건
		public StateGroup TrueState; // 조건이 true일때 전환되는 상태
		public StateGroup FalseState; // 조건이 false일때 전환되는 상태
	}
}
