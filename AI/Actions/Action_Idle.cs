using System.Collections;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;


namespace NetworkAI
{
	public class Action_Idle : StateAction
	{
		public override bool IsLock => false;

		public override void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
		}

		public override void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = (Creature_StateController)controller;
			var entity = creatureController.Entity;

			entity.Server_ProxyAnimationState.Data = AnimationType.Idle_Front;
			entity.Server_StopAgent();
		}

		public override void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
		}
	}
}
