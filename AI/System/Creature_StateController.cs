using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using UnityEngine;
using UnityEngine.AI;

using Sirenix.OdinInspector;
using KaNet.Synchronizers;
using Gameplay;
using Utils;

namespace NetworkAI
{
	[RequireComponent(typeof(Entity_Creature))]
	public class Creature_StateController : StateController
	{
		[field: SerializeField, Header("조종자")]
		public Entity_Creature Entity { get; protected set; }

		[field: Header("판단 변수")]
		[field: SerializeField]
		public EntityBase TargetEntity { get; set; }

		[field: SerializeField] public float AttackSpeed = 1.0f;
		[field: SerializeField] public float DealEventTime = 0.5f;
		public float AttackElapsed { get; set; } = 0f;

		public override void OnValidate()
		{
			base.OnValidate();
			Entity = GetComponent<Entity_Creature>();
		}

		public override void OnInitialize(DeltaTimeInfo deltaTimeInfo)
		{
			base.OnInitialize(deltaTimeInfo);

			TargetEntity = null;
		}
	}
}
