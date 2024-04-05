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

public class DataHandler : NetworkObject
{
	/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
	public override NetObjectType Type => NetObjectType.System_DataHandler;

	private GameplayManager mGameplayManager;

	// Player Character Data
	private Dictionary<CharacterType, PlayerCharacterData> mInitialPlayerCharacterData = new();

	// Enemy Data
	[SerializeField]
	private List<EntityData> InitialEnemyDataList = new();
	private Dictionary<NetObjectType, EntityData> mInitialEnemyDataTable = new();

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	public bool TryGetPlayerCharacterData(CharacterType characterType, out PlayerCharacterData data)
	{
		return mInitialPlayerCharacterData.TryGetValue(characterType, out data);
	}

	public bool TryGetEquipmentData(EquipmentType equipmentType, out EquipmentData data)
	{
		return GlobalServiceLocator
			.ResourcesService
			.GetServiceOrNull()
			.EquipmentDataTable
			.TryGetValue(equipmentType, out data);
	}

	public override void Server_OnStart()
	{
		#region Player Character Data

		// Police
		var police = new PlayerCharacterData();

		police.CharacterType = CharacterType.Police;
		police.MaxHp = 100;
		police.MoveSpeed = 5.0F;
		TryGetEquipmentData(EquipmentType.PumpActionShotgun, out police.Primary);
		TryGetEquipmentData(EquipmentType.SubmachineGun, out police.Secondary);
		TryGetEquipmentData(EquipmentType.StunGrenade, out police.Auxillary);

		addCharacterData(police);

		// Soldier
		var soldier = new PlayerCharacterData();

		soldier.CharacterType = CharacterType.Soldier;
		soldier.MaxHp = 100;
		soldier.MoveSpeed = 6.0F;
		TryGetEquipmentData(EquipmentType.LightMachineGun, out soldier.Primary);
		TryGetEquipmentData(EquipmentType.PistolGrenadeLauncher, out soldier.Secondary);
		TryGetEquipmentData(EquipmentType.Steampack, out soldier.Auxillary);

		addCharacterData(soldier);

		// Sniper
		var sniper = new PlayerCharacterData();

		sniper.CharacterType = CharacterType.Sniper;
		sniper.MaxHp = 100;
		sniper.MoveSpeed = 4.0F;
		TryGetEquipmentData(EquipmentType.SniperRifle, out sniper.Primary);
		TryGetEquipmentData(EquipmentType.HeavyPistol, out sniper.Secondary);
		TryGetEquipmentData(EquipmentType.FreezeGrenade, out sniper.Auxillary);

		addCharacterData(sniper);

		// Engineer
		var engineer = new PlayerCharacterData();

		engineer.CharacterType = CharacterType.Engineer;
		engineer.MaxHp = 100;
		engineer.MoveSpeed = 4.5F;
		TryGetEquipmentData(EquipmentType.LaserRifle, out engineer.Primary);
		TryGetEquipmentData(EquipmentType.TaserGun, out engineer.Secondary);
		TryGetEquipmentData(EquipmentType.HealDrone, out engineer.Auxillary);

		addCharacterData(engineer);

		#endregion

		mInitialEnemyDataTable = new();

		foreach (var data in InitialEnemyDataList)
		{
			if (!mInitialEnemyDataTable.TryAdd(data.EntityType, data))
			{
				Ulog.LogError(this, $"There is duplicated entity data in {data.EntityType}");
			}
		}

		void addCharacterData(PlayerCharacterData data)
		{
			if (!mInitialPlayerCharacterData.TryAddUniqueByKey(data.CharacterType, data))
			{
				Ulog.LogError(this, $"There is duplicated player data in {data.CharacterType}");
			}
		}
	}

	public void OnGameStart()
	{
	}

	public bool TryGetInitialPlayerData(CharacterType characterType, PlayerCharacterData data)
	{
		return mInitialPlayerCharacterData.TryGetValue(characterType, out data);
	}

	public bool TryGetInitialEnemyData(NetObjectType type, out EntityData entityData)
	{
		return mInitialEnemyDataTable.TryGetValue(type, out entityData);
	}
}

