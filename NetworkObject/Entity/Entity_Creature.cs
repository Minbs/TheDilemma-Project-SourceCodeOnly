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

public class Entity_Creature : Entity_Remote
{
	[field : SerializeField] public virtual Proxy_CreatureEntity Proxy_Unit { get; protected set; }
	[field: SerializeField] public virtual EntityNavigation EntityNavigation { get; protected set; }
	[field: SerializeField] public Creature_StateController EntityStateController;
	[field : SerializeField] public Vector3 SpawnPosition { get; private set; }

	public override NetObjectType Type => NetObjectType.Entity_Creature;

	public override void OnValidate()
	{
		base.OnValidate();

		Proxy_Unit = GetComponentInChildren<Proxy_CreatureEntity>();
		EntityNavigation = GetComponentInChildren<EntityNavigation>();
		EntityStateController = GetComponentInChildren<Creature_StateController>();
	}

	public override void Common_OnStart()
	{
		base.Common_OnStart();
		SpawnPosition = transform.position;
		Proxy_Unit.Initialize(this);
		EntityNavigation.Initialize(this);
	}

	public override void Server_OnStart()
	{
		base.Server_OnStart();
		EntityStateController.OnInitialize(new DeltaTimeInfo(1));
	}

	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		base.Client_OnUpdate(deltaTimeInfo);
		Proxy_Unit.PlayAnimation(Server_ProxyAnimationState.Data.GetEnum());
		Proxy_Unit.OnUpdate(deltaTimeInfo);
	}

	public override void Server_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		base.Server_OnFixedUpdate(deltaTimeInfo);
		EntityNavigation.OnUpdateDestinaion(deltaTimeInfo);
		EntityStateController.OnUpdate(deltaTimeInfo);
	}

	#region Navigation

	public void Server_SetDestination(Vector3 destination)
	{
		EntityNavigation.SetDestination(destination);
	}

	public void Server_SetDestination(EntityBase target)
	{
		EntityNavigation.SetDestination(target);
	}

	public void Server_StopAgent()
	{
		EntityNavigation.Stop();
	}

	public void Server_WarpAgent(Vector3 position)
	{
		EntityNavigation.Warp(position);
	}

	#endregion

	#region Proxy

	[SyncVar]
	public readonly SyncField<NetUInt8<AnimationType>> Server_ProxyAnimationState = new();

	public void Server_LookDestination()
	{
		RPC_Client_LookAt.Invoke(EntityNavigation.MoveDirection);
	}

	public void Server_LookAt(Transform target)
	{
		var direction = target.position - transform.position;
		RPC_Client_LookAt.Invoke(direction.ToVector2());
	}

	[RpcCall]
	public readonly RpcCaller<NetVector2> RPC_Client_LookAt = new();
	public void Client_LookAt(NetVector2 direction)
	{
		Proxy_Unit.LookAt(direction);
	}

	#endregion

	#region Combat

	public void Server_ActAttack(AttackType type)
	{
		RPC_Client_PerformAttack.Invoke(type);
	}

	[RpcCall]
	public readonly RpcCaller<NetUInt8<AttackType>> RPC_Client_PerformAttack = new();
	public void Client_PerformAttack(NetUInt8<AttackType> attackType)
	{
		onPerformAttack(attackType);
	}

	protected virtual void onPerformAttack(AttackType attackType) {}

	#endregion
}