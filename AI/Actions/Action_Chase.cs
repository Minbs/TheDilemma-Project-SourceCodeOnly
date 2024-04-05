using System.Collections;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


namespace NetworkAI
{
	public class Action_Chase : StateAction
	{
		public override bool IsLock => false;

		public override void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
		}
		
		public override void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = (Creature_StateController)controller;

			var entity = creatureController.Entity;
			var target = creatureController.TargetEntity;

			entity.Server_ProxyAnimationState.Data = AnimationType.Move_Front;
			entity.Server_LookAt(target.transform);
			entity.Server_SetDestination(target);
		}

		public override void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
		}
	}
}
