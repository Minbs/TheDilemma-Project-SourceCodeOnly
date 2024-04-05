using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using PluggableAI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NetworkAI
{
	/// <summary>
	/// 현재 상태에서 또는 다른 상태로 전환될 때 실행되는 함수를 가지는
	/// 추상 클래스입니다.
	/// </summary>  
	public abstract class StateAction : MonoBehaviour
	{
		/// <summary>스테이트를 잠급니다. 잠긴 경우 Condition을 무시합니다.</summary>
		[ShowInInspector]
		public abstract bool IsLock { get; }

		/// <summary>State의 시작시 호출됩니다.</summary>
		/// <param name="controller">실행하는 Controller입니다.</param>
		/// <param name="deltaTimeInfo">
		/// State의 실행 DeltaTime정보입니다.
		/// State의 실행은 Unity Mono의 Event와는 다른, 사용자가 정의한 Interval로 동작합니다.
		/// State의 DeltaTime은 이전의 Event 호출 시점부터 현재 Event시점까지의 실행 간격입니다.
		/// </param>
		public abstract void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo);

		/// <summary>현재 State의 행동을 실행합니다.</summary>
		/// <param name="controller">실행하는 Controller입니다.</param>
		/// <param name="deltaTimeInfo">
		/// State의 실행 DeltaTime정보입니다.
		/// State의 실행은 Unity Mono의 Event와는 다른, 사용자가 정의한 Interval로 동작합니다.
		/// State의 DeltaTime은 이전의 Event 호출 시점부터 현재 Event시점까지의 실행 간격입니다.
		/// </param>
		public abstract void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo);

		/// <summary>State의 종료시 호출됩니다.</summary>
		/// <param name="controller">실행하는 Controller입니다.</param>
		/// <param name="deltaTimeInfo">
		/// State의 실행 DeltaTime정보입니다.
		/// State의 실행은 Unity Mono의 Event와는 다른, 사용자가 정의한 Interval로 동작합니다.
		/// State의 DeltaTime은 이전의 Event 호출 시점부터 현재 Event시점까지의 실행 간격입니다.
		/// </param>
		public abstract void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo);
	}
}
