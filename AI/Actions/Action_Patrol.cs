using System.Collections;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using PluggableAI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

namespace NetworkAI
{
	public class Action_Patrol : StateAction
	{
		[Title("Б¤Вы")]
		[field : SerializeField]
		[MinMaxSlider(0.5F, 10.0F)] public Vector2 PatrolDelayInit = new Vector2(2.0F, 5.0F);
		[field : SerializeField]
		[MinMaxSlider(-20F, 20F)] public Vector2 PatrolRange = new Vector2(-10F, 10F);

		[field : SerializeField] public float PatrolDelay { get; private set; }
		[field : SerializeField] public int TryFindCount { get; private set; } = 20;

		public override bool IsLock => false;

		private float getRandomDelay()
		{
			return Random.Range(PatrolDelayInit.x, PatrolDelayInit.y);
		}

		private Vector2 getRandomRelativePointByRange()
		{
			float x = Random.Range(PatrolRange.x, PatrolRange.y);
			float y = Random.Range(PatrolRange.x, PatrolRange.y);

			return new Vector2(x, y);
		}

		public override void OnStart(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = (Creature_StateController)controller;

			PatrolDelay = getRandomDelay();
		}

		public override void OnAct(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
			var creatureController = controller as Creature_StateController;
			var entity = creatureController.Entity;

			entity.Server_LookDestination();
			entity.Server_ProxyAnimationState.Data = AnimationType.Move_Front;

			// Check patrol delay
			if (PatrolDelay > 0)
			{
				PatrolDelay -= deltaTimeInfo.ScaledDeltaTime;
			}

			// Get destination
			PatrolDelay = getRandomDelay();
			Vector2 initialPosition = entity.SpawnPosition;
			Vector2 destination = initialPosition;

			for (int i = 0; i < TryFindCount; i++)
			{
				var targetPos = initialPosition + getRandomRelativePointByRange();
				if (entity.EntityNavigation.CanReach(targetPos))
				{
					destination = targetPos;
					break;
				}
			}

			entity.Server_SetDestination(destination);
		}

		public override void OnEnd(StateController controller, DeltaTimeInfo deltaTimeInfo)
		{
		}
	}
}