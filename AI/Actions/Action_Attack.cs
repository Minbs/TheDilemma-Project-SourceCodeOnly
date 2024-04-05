using System.Collections;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


namespace NetworkAI
{
	public class Action_Attack : StateAction
	{
		public override bool IsLock => false;
		private bool isDealToTarget = false;

		public override void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			isDealToTarget = false;

			var creatureController = (Creature_StateController)controller;

			creatureController.Entity.Server_StopAgent();
			creatureController.AttackElapsed = 0;

			var entity = creatureController.Entity;
			var target = creatureController.TargetEntity;
			entity.Server_ProxyAnimationState.Data = AnimationType.Attack_Front;
			entity.Server_LookAt(target.transform);
		}

		public override void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = (Creature_StateController)controller;

			creatureController.AttackElapsed += deltaTimeInfo.ScaledDeltaTime;

			if (creatureController.AttackElapsed >= creatureController.AttackSpeed)
			{
				return;
			}

			if (creatureController.AttackElapsed >= creatureController.DealEventTime && !isDealToTarget)
			{
				var entity = creatureController.Entity;
				entity.Server_ActAttack(AttackType.Normal_1);
				isDealToTarget = true;
				Debug.Log("deal");
			}
		}

		public override void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = (Creature_StateController)controller;
			isDealToTarget = false;
			creatureController.AttackElapsed = 0;
			creatureController.Entity.Server_ProxyAnimationState.Data = AnimationType.Idle_Front;
		}
	}
}
