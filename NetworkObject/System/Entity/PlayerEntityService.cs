using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;
using Utils;
using Utils.Service;

public class PlayerEntityService : IServiceable
{
	private GameplayManager mGameplayManager;
	private EntityService mEntityService;

	private BidirectionalMap<NetSessionID, Entity_PlayerController> mPlayerTableBySessionID = new();
	public List<Entity_PlayerController> PlayerList => mPlayerTableBySessionID.ForwardValues.ToList();

	// Event
	public event Action<Entity_PlayerController> OnClientPlayerEntityCreated;
	public event Action<Entity_PlayerController> OnClientPlayerEntityRemoved;

	private Entity_PlayerController mClientPlayer;
	public bool IsClientPlayerExist => mClientPlayer != null;

	public PlayerEntityService(GameplayManager gameplayManager, EntityService entityService)
	{
		mGameplayManager = gameplayManager;
		mEntityService = entityService;

		mEntityService.OnEntityCreated += (entity) =>
		{
			var player = entity as Entity_PlayerController;

			// If is player
			if (player != null)
			{
				onPlayerEntityCreated(player);
			}
		};

		mEntityService.OnEntityRemoved += (entity) =>
		{
			var player = entity as Entity_PlayerController;

			// If is player
			if (player != null)
			{
				onPlayerEntityReleased(player);
			}
		};

		// 나중에 개선이 필요함
		OnClientPlayerEntityCreated += (player) =>
		{
			mGameplayManager.PlayerCamera.SetTarget(player.CameraTarget);
			mClientPlayer = player;
		};

		OnClientPlayerEntityRemoved += (player) =>
		{
			mClientPlayer = null;
		};
	}

	public void OnRegistered()
	{
	}

	public void OnUnregistered()
	{
	}

	public void Server_SpawnPlayer(NetSessionID spawnPlayer, Vector3 spawnPosition)
	{
		if (mPlayerTableBySessionID.ContainsForward(spawnPlayer))
		{
			Ulog.LogError(this, $"You try to create same player entity! Owner : {spawnPlayer}");
			return;
		}

		mGameplayManager.ObjectManager.CreateNetworkObjectAsServer<Entity_PlayerController>
		(
			NetObjectType.Entity_Player,
			spawnPlayer,
			spawnPosition,
			Quaternion.identity
		);
	}

	public void onPlayerEntityCreated(EntityBase playerEntity)
	{
		var player = playerEntity as Entity_PlayerController;
		if (player == null)
		{
			Ulog.LogError(this, $"It's not player entity.");
			return;
		}

		if (mPlayerTableBySessionID.ContainsForward(player.OwnerID))
		{
			return;
		}

		// Check it's client's entity
		if (playerEntity.OwnerID == mGameplayManager.ClientID)
		{
			OnClientPlayerEntityCreated?.Invoke(player);
		}

		mPlayerTableBySessionID.Add(player.OwnerID, player);
	}

	private void onPlayerEntityReleased(EntityBase playerEntity)
	{
		var player = playerEntity as Entity_PlayerController;
		if (player == null)
		{
			Ulog.LogError(this, $"It's not player entity.");
			return;
		}

		if (!mPlayerTableBySessionID.ContainsForward(player.OwnerID))
		{
			return;
		}

		// Check it's client's entity
		if (player.OwnerID == mGameplayManager.ClientID)
		{
			OnClientPlayerEntityRemoved?.Invoke(player);
		}

		mPlayerTableBySessionID.TryRemove(player);
	}

	public void KillPlayer(NetSessionID sessionID)
	{
		if (mPlayerTableBySessionID.TryGetForward(sessionID, out var player))
		{
			player.Kill();
		}
	}

	public bool IsPlayerExist(NetSessionID sessionID)
	{
		return mPlayerTableBySessionID.Contains(sessionID);
	}

	/// <summary>생존해 있는 Player Entity를 반환합니다.</summary>
	/// <param name="sessionID">PlayerEntity를 소유한 Session ID입니다.</param>
	public bool TryGetPlayer(NetSessionID sessionID, out Entity_PlayerController player)
	{
		return mPlayerTableBySessionID.TryGetValue(sessionID, out player);
	}
	
	public bool TryGetClientPlayer(out Entity_PlayerController clientPlayer)
	{
		if (IsClientPlayerExist)
		{
			clientPlayer = mClientPlayer;
			return true;
		}

		clientPlayer = null;
		return false;
	}

	public int GetAlivePlayerCount()
	{
		return mPlayerTableBySessionID.Count;
	}

	#region Process Input
	public void ProcessHealSelf()
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessHealSelf();
		}
	}

	public void ProcessInteract()
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessInteract();
		}
	}

	public void ProcessEquipmentSwapInput(int index)
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessEquipmentSwapInput(index);
		}
	}

	public void ProcessReload()
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessReload();
		}
	}

	public void ProcessMovementInput(NetVector2 moveInput)
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessMovementInput(moveInput);
		}
	}

	public void ProcessMousePressed(bool isLeft)
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessMousePressed(isLeft);
		}
	}

	public void ProcessMousePressing(bool isLeft)
	{
		if (IsClientPlayerExist)
		{
			mClientPlayer.ProcessMousePressing(isLeft);
		}
	}
	#endregion
}