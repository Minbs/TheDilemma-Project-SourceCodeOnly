using System.Collections;
using System.Collections.Generic;
using Gameplay;
using JetBrains.Annotations;
using KaNet.Synchronizers;
using PluggableAI;
using UnityEngine;

namespace NetworkAI
{
	public class Condition_MeleeAttackTarget : StateCondition
	{
		[field: SerializeField] public FactionMatchType FactionMatch { get; private set; }
		[field: SerializeField] public float Radius { get; private set; } = 5;

		public override bool CheckCondition(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = controller as Creature_StateController;
			var entity = creatureController.Entity;

			if (creatureController.AttackElapsed >= creatureController.AttackSpeed)
			{
				return false;
			}

			if (CreatureStateSensor.TryGetTargetInCircleRange
			(
				entity,
				FactionMatch,
				Radius,
				out var target
			))
			{
				creatureController.TargetEntity = target;
				return true;
			}

			if (entity.Server_ProxyAnimationState.Data == AnimationType.Attack_Front)
			{
				return true;
			}

			return false;
		}
	}
}
