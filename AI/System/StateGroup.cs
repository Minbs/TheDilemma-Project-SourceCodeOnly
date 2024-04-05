using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KaNet.Synchronizers;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using Utils;

namespace NetworkAI
{
	/// <summary>행동할 Action과 전환 조건의 집합 그룹입니다.</summary>
	public class StateGroup : MonoBehaviour
	{
		[SerializeField] private StateAction mAction; // 현재 상태에서 실행하는 행동
		[SerializeField] private StateTransition[] mTransitions; // 현재 상태에서 전환될 수 있는 상태와 조건

		/// <summary>현재 State가 동작한 경과시간입니다.</summary>
		public float StateElapsedSec { get; private set; }

		/// <summary>스테이트를 업데이트합니다.</summary>
		/// <param name="controller"></param>
		/// <param name="deltaTimeInfo"></param>
		public void OnUpdate(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			OnAct(controller, deltaTimeInfo);

			if (!HasLock())
			{
				if (TryGetNextStateByConditions(controller, deltaTimeInfo, out var nextState))
				{
					// 전환 조건을 만족하는 State가 있다면 전환합니다.
					controller.ChangeState(deltaTimeInfo, nextState);
					return;
				}
			}
		}

		/// <summary>전환 조건을 만족하는 다음 State를 반환합니다.</summary>
		/// <param name="nextState">전환이 결정된 다음 State입니다.</param>
		/// <returns>전환되었으면 true를 반환합니다.</returns>
		private bool TryGetNextStateByConditions(StateController controller, DeltaTimeInfo deltaTimeInfo, out StateGroup nextState)
		{
			//var previousState = controller.CurrentStateGroup;
			var previousState = controller.CurrentStateGroup;

			foreach (var transition in mTransitions)
			{
				bool checkCondition = transition.Condition.CheckCondition(controller, deltaTimeInfo);

				nextState = checkCondition ? transition.TrueState : transition.FalseState;

				if (controller.CurrentStateGroup.Equals(nextState))
				{
					continue;
				}

				return true;
			}

			nextState = null;
			return false;
		}

		public bool HasLock()
		{
			return mAction.IsLock;
		}

		public void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			StateElapsedSec = 0.0F;
			mAction.OnStart(controller, deltaTimeInfo);
		}

		public void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			StateElapsedSec += deltaTimeInfo.ScaledDeltaTime;

			mAction.OnAct(controller, deltaTimeInfo);
		}

		public void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			StateElapsedSec = 0.0F;
			mAction.OnEnd(controller, deltaTimeInfo);
		}

		public override string ToString() => name;
	}
}
