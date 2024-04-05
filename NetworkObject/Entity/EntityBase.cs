using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;

using Sirenix.OdinInspector;
using Gameplay;

public class EntityBase : NetworkObject
{
	public override NetObjectType Type => NetObjectType.Entity;

	// DI
	protected GameplayManager mGameplayManager;

	// Entity Basic Component
	[field : SerializeField] public Rigidbody2D EntityRigid { get; private set; }

	// Character Definition
	[SyncVar] protected readonly SyncField<NetUInt8<FactionType>> mFaction = new();
	[TitleGroup("Entity")][ShowInInspector] public FactionType Faction => mFaction.Data.GetEnum();

	[SyncVar] protected readonly SyncField<NetInt32> mMaxHP = new();
	[TitleGroup("Entity")][ShowInInspector] public int MaxHP => mMaxHP.Data;

	[SyncVar] protected readonly SyncField<NetInt32> mHP = new();
	[TitleGroup("Entity")][ShowInInspector] public int HP => mHP.Data;

	[SyncVar] protected readonly SyncField<NetBool> mIsAlive = new();
	[TitleGroup("Entity")][ShowInInspector] public bool IsAlive => mIsAlive.Data;

	[SyncVar] protected readonly SyncField<NetFloat> mMoveSpeed = new();
	[TitleGroup("Entity")][ShowInInspector] public float MoveSpeed => mMoveSpeed.Data;

	public virtual void OnValidate()
	{
		EntityRigid = GetComponent<Rigidbody2D>();
	}

	public virtual void InitializeByHandler(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	public virtual void Server_InitializeByEntityService(EntityData entityData, FactionType faction)
	{
		// Setup basic values
		mFaction.Data = faction;
		mMaxHP.Data = entityData.HP;
		mHP.Data = entityData.HP;
		mMoveSpeed.Data = entityData.MoveSpeed;
		mIsAlive.Data = true;
	}

	public virtual void Kill()
	{
		mHP.Data = 0;
		mIsAlive.Data = false;
		OnDeath();
	}

	public virtual void OnDeath()
	{
		Release();
	}

	/// <summary>피격당했을 때의 이벤트입니다.</summary>
	/// <param name="hitscanInfo">Hitscan 정보입니다.</param>
	/// <param name="hasAuthority">권한 여부입니다. false라면 실제 데미지가 반영되지 않습니다.</param>
	public virtual void OnHitBy
	(
		HitscanInfo hitscanInfo, 
		RaycastHit2DInfo raycast, 
		bool hasAuthority
	)
	{
		if (hasAuthority)
		{
			//bool isAlliance = hitscanInfo.Attacker.Faction.GetEnum().IsAlliance(Faction);

			//float multiply = raycast.HitboxDamageMultiply;
			//if (isAlliance)
			//{
			//	multiply *= GlobalGameplayData.FRIENDLY_FIRE_RATIO;
			//}

			//int damage = (int)(hitscanInfo.WeaponInfo.Damage * multiply);
			//OnDamage(damage);

			OnHitBy
			(
				hitscanInfo.Attacker.Faction.GetEnum(), 
				raycast.HitboxDamageMultiply,
				hitscanInfo.WeaponInfo.Damage
			);
		}
	}

	public void OnHitBy
	(
		FactionType attackerFaction, 
		float hitboxDamageMultiply,
		int damage
	)
	{
		bool isAlliance = attackerFaction.IsAlliance(Faction);
		float multiply = hitboxDamageMultiply;
		if (isAlliance)
		{
			multiply *= GlobalGameplayData.FRIENDLY_FIRE_RATIO;
		}

		int performDamage = (int)(damage * multiply);
		OnDamage(performDamage);
	}

	public void OnDamage(int damage)
	{
		mHP.Data -= damage;
		if (mHP.Data <= 0)
		{
			Kill();
		}
	}
}
