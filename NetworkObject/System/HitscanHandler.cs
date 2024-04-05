using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Gameplay;

public class HitscanHandler : NetworkObject
{
	/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
	public override NetObjectType Type => NetObjectType.System_HitscanHandler;

	private GameplayManager mGameplayManager;
	public EffectHandler EffectHandler => mGameplayManager.EffectHandler;

	private BidirectionalMap<NetObjectID, HitscanBase> mHitscanInstanceTable = new();

	/// <summary>유저가 소유할 수 있는 Index의 범위입니다.</summary>
	public const int CLIENT_OCCUPY_INDEX_OFFSET = ushort.MaxValue / KaNetGlobal.SYSTEM_MAX_PLAYER;

	private int mClientHitscanID = 0;

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	public override void Common_OnStart()
	{
		resetClientHitscanID();
	}

	private void resetClientHitscanID()
	{
		mClientHitscanID = ObjectManager.CurrentClientID * CLIENT_OCCUPY_INDEX_OFFSET;
	}

	public NetObjectID GetNewHitscanIdByClient()
	{
		var clientID = ObjectManager.CurrentClientID;

		mClientHitscanID++;

		for (int i = 0; i < CLIENT_OCCUPY_INDEX_OFFSET; i++)
		{
			if (mClientHitscanID >= (clientID + 1) * CLIENT_OCCUPY_INDEX_OFFSET)
			{
				resetClientHitscanID();
			}

			if (mHitscanInstanceTable.Contains((ushort)mClientHitscanID))
			{
				mClientHitscanID++;
			}
			else
			{
				return (ushort)mClientHitscanID;
			}
		}

		Ulog.LogError(this, $"There is no hitscan id remaining!");
		return 0;
	}

	public override void Server_OnStart()
	{
		Ulog.Log(this, "Start as server");
	}

	#region Process

	/// <summary>클라이언트로써 피격 판정 허용을 요청합니다. 자신이 발사한 객체가 아니라면 무시됩니다.</summary>
	public void Client_RequestPerform(NetObjectID hitscanID, HitscanInfo hitscanInfo)
	{
		if (hitscanInfo.AuthorityID == ClientID)
		{
			RPC_Server_ProcessHitscan.Invoke(ClientID, hitscanID, hitscanInfo);
		}
	}
	
	[RpcCall(SyncType.ReliableInstant, SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetObjectID, HitscanInfo> RPC_Server_ProcessHitscan = new();
	public void Server_ProcessHitscan(NetSessionID sender, NetObjectID hitscanID, HitscanInfo hitscanInfo)
	{
		if (!IsServerSide)
		{
			return;
		}

		var broadcastSessions = this.ObjectManager.GetNetSessionIDsExcept(sender);
		RPC_Client_ReleaseByID.Invoke(hitscanID, broadcastSessions);

		if (mHitscanInstanceTable.TryGetValue(hitscanID, out var hitscan))
		{
			hitscan.OnPerformed(hitscanInfo);
			mHitscanInstanceTable.TryRemove(hitscanID);
		}

		//bool hasAuthority = hitscanInfo.AuthorityID == ObjectManager.CurrentClientID;

		foreach (var hit in hitscanInfo.HitList.DataList)
		{
			if (ObjectManager.TryFindObjectsByID(hit.Target, out var networkObject))
			{
				var entity = networkObject as EntityBase;
				entity?.OnHitBy(hitscanInfo, hit, true);
			}
		}
	}

	#endregion

	#region Hitscan Synchronize

	/// <summary>자신이 소유한 Hitscan을 생성합니다. 생성한 클라이언트가 판단합니다.</summary>
	public void CreateHitscanAsOwner
	(
		EntityBase attacker,
		WeaponInfo weaponInfo,
		Vector2 origin,
		Vector2 direction
	)
	{
		HitscanInfo hitscanInfo = new HitscanInfo
		(
			ClientID,
			attacker,
			weaponInfo,
			origin,
			direction
		);

		switch (hitscanInfo.WeaponInfo.HitscanType.GetEnum().GetBaseType())
		{
			case BaseHitscanType.Instant:
				if (!TryCreateHitscan(hitscanInfo, origin, -1))
				{
					Ulog.LogError(this, $"Instant hitscan create error!");
					return;
				}
				break;

			case BaseHitscanType.Simulate:
				var newID = GetNewHitscanIdByClient();
				Client_CreateHitscanByServer(hitscanInfo, origin, newID);
				RPC_Server_CreateRequestHitscan.Invoke(ClientID, hitscanInfo, origin, newID);
				break;

			case BaseHitscanType.Synchronize:
				break;
		}
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, HitscanInfo, NetVector2, NetObjectID> RPC_Server_CreateRequestHitscan = new();
	public void Server_CreateRequestHitscan(NetSessionID requestClientID, HitscanInfo hitscanInfo, NetVector2 origin, NetObjectID objectID)
	{
		var broadcastSessions = ObjectManager.GetNetSessionIDsExcept(requestClientID);
		if (broadcastSessions.Length > 0)
		{
			RPC_Client_CreateHitscanByServer.Invoke(hitscanInfo, origin, objectID, broadcastSessions);
		}
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.ServerOnly)]
	private readonly RpcCaller<HitscanInfo, NetVector2, NetObjectID> RPC_Client_CreateHitscanByServer = new();
	public void Client_CreateHitscanByServer(HitscanInfo hitscanInfo, NetVector2 origin, NetObjectID objectID)
	{
		TryCreateHitscan(hitscanInfo, origin, objectID);
	}

	#endregion

	#region	Creation Deletion

	/// <summary>히트스캔을 생성합니다.</summary>
	public bool TryCreateHitscan
	(
		HitscanInfo hitscanInfo, 
		Vector2 spawnPosition,
		int hitscanID
	)
	{
		var type = hitscanInfo.WeaponInfo.HitscanType;

		var resources = GlobalServiceLocator.ResourcesService.GetServiceOrNull();
		if (!resources.HitscanPrefabTable.TryGetValue(type, out var hitscanPrefab))
		{
			Ulog.LogError(this, $"There is no such hitscan object as {type}");
			return false;
		}

		if (mHitscanInstanceTable.Contains((ushort)hitscanID))
		{
			return false;
		}

		var hitscanInst = GlobalServiceLocator
			.MonoObjectPoolService
			.GetServiceOrNull()
			.CreateObject(hitscanPrefab, spawnPosition, Quaternion.identity);

		var hitscan = hitscanInst.GetComponent<HitscanBase>();

		hitscan.Initialize(this, hitscanInfo, (ushort)hitscanID);

		if (hitscan.Type.GetBaseType() == BaseHitscanType.Instant)
		{
			return true;
		}

		// HitscanID가 음수라면 등록하지 않는다. 클라이언트측에서 즉시 판단됨
		if (hitscanID < 0)
		{
			return false;
		}

		if (!mHitscanInstanceTable.TryAdd(hitscan, hitscan.HitscanID))
		{
			Ulog.LogError(this, $"Hitscan {hitscan.HitscanID} is already exist");
			return false;
		}

		return true;
	}

	[RpcCall]
	private RpcCaller<NetObjectID> RPC_Client_ReleaseByID = new();
	private void Client_ReleaseByID(NetObjectID hitscanID)
	{
		if (mHitscanInstanceTable.TryGetValue(hitscanID, out var hitscan))
		{
			GlobalServiceLocator
				.MonoObjectPoolService
				.GetServiceOrNull()
				.Release(hitscan.gameObject);

			mHitscanInstanceTable.TryRemove(hitscan.HitscanID);
		}
	}

	public void Release(HitscanBase hitscan)
	{
		GlobalServiceLocator
			.MonoObjectPoolService
			.GetServiceOrNull()
			.Release(hitscan.gameObject);

		mHitscanInstanceTable.TryRemove(hitscan.HitscanID);
	}

	#endregion
}

