using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Utils.ViewModel;
using Gameplay;
using NetworkAI;

public class Entity_DummyEnemy : Entity_Creature
{
	public override NetObjectType Type => NetObjectType.Entity_DummyEnemy;

	[field : Header("АјАн")]
	[SerializeField] private LocalHitscanBase Attack_Normal_1;

	protected override void onPerformAttack(AttackType attackType)
	{
		Attack_Normal_1.Perform(IsServerSide);
	}
}
