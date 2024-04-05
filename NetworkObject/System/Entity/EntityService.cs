using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;
using Utils;
using Utils.Service;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.EventSystems.EventTrigger;

namespace Gameplay
{
	public class EntityService : IServiceable
	{
		// DI
		private GameplayManager mGameplayManager;

		// Entity Services
		public PlayerEntityService PlayerEntityService { get; private set; }

		private Dictionary<NetObjectID, EntityBase> mEntityTable = new();

		public event Action<EntityBase> OnEntityCreated;
		public event Action<EntityBase>	OnEntityRemoved;

		public EntityService(GameplayManager gameplayManager)
		{
			mGameplayManager = gameplayManager;
			PlayerEntityService = new PlayerEntityService(gameplayManager, this);

			mGameplayManager.IngameSessionHandler.OnSessionDisconnected += OnSessionDisconnected;
		}

		public void OnRegistered()
		{
			mGameplayManager.ObjectManager.OnCreated += onCreated;
		}

		private void onCreated(NetworkObject obj)
		{
			if (!obj.Type.IsBaseType(BaseNetObjectType.Entity))
			{
				return;
			}

			var entity = obj as EntityBase;
			if (entity == null)
			{
				return;
			}

			if (!mEntityTable.TryAddUniqueByKey(obj.ID, entity))
			{
				return;
			}

			entity.InitializeByHandler(mGameplayManager);
			entity.OnRelease += onReleaseEntity;
			OnEntityCreated?.Invoke(entity);
		}

		public void OnUnregistered()
		{
		}

		private void onReleaseEntity(NetworkObject entityObject)
		{
			OnEntityRemoved?.Invoke(entityObject as EntityBase);
			mEntityTable.Remove(entityObject.ID);
		}

		private void OnSessionDisconnected(NetSessionID session)
		{
			PlayerEntityService.KillPlayer(session);
		}

		public void Server_SpawnPlayer
		(
			NetSessionID spawnPlayer,
			Vector3 spawnPosition,
			PlayerCharacterData characterData
		)
		{
			if (!mGameplayManager.IsServerSide)
			{
				Ulog.LogError(this, $"Server_SpawnPlayer Error! You're not server!");
				return;
			}

			if (PlayerEntityService.TryGetPlayer(spawnPlayer, out var existPlayer))
			{
				// 플레이어가 살아있다면 스폰 위치로 이동시킨다.
				existPlayer.RPC_Server_Teleport.Invoke(spawnPosition.ToVector2());
				return;
			}

			var player = mGameplayManager.ObjectManager
				.CreateNetworkObjectAsServer<Entity_PlayerController>
			(
				NetObjectType.Entity_Player,
				spawnPlayer,
				spawnPosition,
				Quaternion.identity
			);

			player.Server_InitializePlayer(characterData, FactionType.Human);
		}

		public void Server_SpawnEntity
		(
			NetObjectType netObjectType, 
			Vector2 spawnPosition, 
			FactionType faction
		)
		{
			if (!mGameplayManager.IsServerSide)
			{
				Ulog.LogError(this, $"Server_SpawnPlayer Error! You're not server!");
				return;
			}

			var enemy = mGameplayManager.ObjectManager
				.CreateNetworkObjectAsServer<EntityBase>
			(
				netObjectType,
				mGameplayManager.ObjectManager.CurrentClientID,
				spawnPosition,
				Quaternion.identity
			);

			if (mGameplayManager.DataHandler.TryGetInitialEnemyData(netObjectType, out var enemyData))
			{
				enemy.Server_InitializeByEntityService(enemyData, faction);
			}
			else
			{
				Ulog.LogError(this, $"There is no \"{netObjectType}\"'s entity data!");
			}
		}

		public bool TryGetEntity(NetObjectID entityID, out EntityBase entity)
		{
			return mEntityTable.TryGetValue(entityID, out entity);
		}

		public void KillEntity(bool includePlayer)
		{
			var entities = mEntityTable.Values;

			foreach (var e in entities.ToList())
			{
				if (includePlayer || e.Type != NetObjectType.Entity_Player)
				{
					e.Release();
				}
			}
		}
	}
}
