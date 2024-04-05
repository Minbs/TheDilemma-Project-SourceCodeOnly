using System.Collections;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using PluggableAI;
using UnityEngine;

namespace NetworkAI
{
	public class Condition_ChaseTargetOnCircle : StateCondition
	{
		[field: SerializeField] public FactionMatchType FactionMatch { get; private set; }
		[field: SerializeField] public float Radius { get; private set; } = 5;

		public override bool CheckCondition(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = controller as Creature_StateController;
			var entity = creatureController.Entity;

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

			return false;
		}
	}
}
