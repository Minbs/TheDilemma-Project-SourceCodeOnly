using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Gameplay;
using UnityEngine.UIElements;

public class EffectHandler : NetworkObject
{
	/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
	public override NetObjectType Type => NetObjectType.System_EffectHandler;

	private GameplayManager mGameplayManager;

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	#region Normal Effect

	/// <summary>동기화되는 Effect를 생성합니다.</summary>
	public void CreateSyncEffect
	(
		NetUInt16<EffectType> type,
		NetVector2 position,
		Net2dRotation rotation
	)
	{
		createEffect(type, position, rotation);
		RPC_Server_RequestCreateEffect.Invoke(ClientID, type, position, rotation);
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetUInt16<EffectType>, NetVector2, Net2dRotation>
		RPC_Server_RequestCreateEffect = new();
	private void Server_RequestCreateEffect
	(
		NetSessionID caller,
		NetUInt16<EffectType> type,
		NetVector2 position,
		Net2dRotation rotation
	)
	{
		var sendTo = ObjectManager.GetNetSessionIDsExcept(caller);
		RPC_Client_CreateEffect.Invoke(caller, type, position, rotation, sendTo);
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.ServerOnly)]
	private readonly RpcCaller<NetSessionID, NetUInt16<EffectType>, NetVector2, Net2dRotation>
		RPC_Client_CreateEffect = new();
	private void Client_CreateEffect
	(
		NetSessionID caller,
		NetUInt16<EffectType> type, 
		NetVector2 position,
		Net2dRotation rotation
	)
	{
		createEffect(type, position, rotation);
	}

	private void createEffect
	(
		NetUInt16<EffectType> type,
		NetVector2 position,
		Net2dRotation rotation
	)
	{
		if (GlobalServiceLocator
			.ResourcesService
			.GetServiceOrNull()
			.EffectPrefabTable
			.TryGetValue(type, out var effectPrefab))
		{
			GlobalServiceLocator
				.MonoObjectPoolService
				.GetServiceOrNull()
				.CreateObject(effectPrefab, position, rotation);
		}
	}

	#endregion

	#region Hitscan

	/// <summary>동기화되는 Effect를 생성합니다.</summary>
	public void CreateSyncHitscanEffect
	(
		NetUInt16<EffectType> type,
		NetVector2 start,
		NetVector2 end
	)
	{
		createHitscanEffect(type, start, end);
		RPC_Server_RequestCreateHitscanEffect.Invoke(ClientID, type, start, end);
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetUInt16<EffectType>, NetVector2, NetVector2>
		RPC_Server_RequestCreateHitscanEffect = new();
	private void Server_RequestCreateHitscanEffect
	(
		NetSessionID caller,
		NetUInt16<EffectType> type,
		NetVector2 start,
		NetVector2 end
	)
	{
		var sendTo = ObjectManager.GetNetSessionIDsExcept(caller);
		RPC_Client_CreateHitscanEffect.Invoke(caller, type, start, end, sendTo);
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.ServerOnly)]
	private readonly RpcCaller<NetSessionID, NetUInt16<EffectType>, NetVector2, NetVector2>
		RPC_Client_CreateHitscanEffect = new();
	private void Client_CreateHitscanEffect
	(
		NetSessionID caller,
		NetUInt16<EffectType> type,
		NetVector2 start,
		NetVector2 end
	)
	{
		createHitscanEffect(type, start, end);
	}

	private void createHitscanEffect
	(
		NetUInt16<EffectType> type,
		NetVector2 start,
		NetVector2 end
	)
	{
		if (GlobalServiceLocator
			.ResourcesService
			.GetServiceOrNull()
			.EffectPrefabTable
			.TryGetValue(type, out var effectPrefab))
		{
			var hitscanInstance = GlobalServiceLocator
				.MonoObjectPoolService
				.GetServiceOrNull()
				.CreateObject(effectPrefab, start, Quaternion.identity);

			var effect = hitscanInstance.GetComponent<Effect_HitscanBase>();
			if (effect != null)
			{
				effect.InitializeHitscan(start, end);
			}
		}
	}

	#endregion
}

