using Gameplay;
using KaNet;
using KaNet.Synchronizers;
using MonoGUI;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Utils.Service;

public class ResourcesService : MonoService
{
	#region Unity Objects
	[field: SerializeField]
	public SerializableDictionary<NetObjectType, GameObject> NetworkObjectTable { get; private set; }

	[field: SerializeField]
	public SerializableDictionary<MapType, GameObject> MapTable { get; private set; }

	//[field: SerializeField]
	//public SerializableDictionary<ItemType, Sprite> ItemSpriteTable { get; private set; }

	[field: SerializeField]
	public SerializableDictionary<EffectType, GameObject> EffectPrefabTable { get; private set; }

	[field: SerializeField]
	public SerializableDictionary<HitscanType, GameObject> HitscanPrefabTable { get; private set; }

	[field: SerializeField] public List<GameObject> GuiPrefabList { get; private set; }
	public Dictionary<Type, GameObject> GuiTable { get; private set; }
	#endregion

	#region Gameplay Data
	//[field: SerializeField] public SerializableDictionary<ItemType, ItemBase> ItemTable { get; private set; }
	[field: SerializeField] public SerializableDictionary<CharacterType, Sprite> ProfileTable { get; private set; }
	[field: SerializeField] public SerializableDictionary<AmmoInfoType, Sprite> AmmoInfoTable { get; private set; }
	[Title("Weapon Data")]
	public List<EquipmentData> mEquipmentDataList;
	#endregion

	public Dictionary<EquipmentType, EquipmentData> EquipmentDataTable { get; private set; } = new();

	// Terminal Screen
	[SerializeField] private Texture2D mDefaultTerminalScreen;
	public Texture2D TerminalScreen => mDefaultTerminalScreen;

	public async void Start()
	{
		var loadTexture = await ImageProcesser.LoadDesktopBackgroundTextureOrNull(240, 135);
		mDefaultTerminalScreen = loadTexture ?? mDefaultTerminalScreen;
	}

	public override void OnRegistered()
	{
		base.OnRegistered();

		// Bind GUI Prefabs
		GuiTable = new();
		foreach (var guiObject in GuiPrefabList)
		{
			var guiComponent = guiObject.GetComponent<MonoGUI_View>();
			if (guiComponent != null)
			{
				if (!GuiTable.TryAddUniqueByKey(guiComponent.GetType(), guiObject))
				{
					Ulog.LogError(this, $"There is duplicated gui object type exist in {guiObject.name}, " +
						$"Class : {guiComponent.GetType().Name}" + $"PrefabName: {GuiTable[guiComponent.GetType()].name}" + $"Add GameObject: {guiObject.name}");
					return;
				}
			}
		}

		ParseEquipmentCSV(@"data_equipment");

		// Bind Equipment Data
		foreach (var equipmentData in mEquipmentDataList)
		{
			if (!EquipmentDataTable.TryAdd(equipmentData.Equipment, equipmentData))
			{
				Ulog.LogError(this, $"Bind equipment data error at type {equipmentData.Equipment}");
			}
		}

		Ulog.Log(this, $"Bind equipment data! Count : {EquipmentDataTable.Count}");
	}

	public void ParseEquipmentCSV(string relativePath)
	{
		mEquipmentDataList = new();

		var equipmentData = CSVReader.Read(relativePath);

		for (int i = 0; i < equipmentData.Count; i++)
		{
			var table = equipmentData[i];
			try
			{
				mEquipmentDataList.Add(parseAsEquipmentData(table));
			}
			catch (DataParseError e)
			{
				Ulog.LogError(this, $"Parse error in index {i} : {e}");
			}
			catch
			{
				Ulog.LogError(this, $"Parse error in index {i}");
			}
		}

		Ulog.Log(this, $"Equipment parsed! Count : {mEquipmentDataList.Count}");

		EquipmentData parseAsEquipmentData(Dictionary<string, object> dataTable)
		{
			#region Equipment Data
			EquipmentData data = new EquipmentData();

			// Equipment
			if (Enum.TryParse<EquipmentType>(dataTable["Equipment"].ToString(), true, out var equipmentType))
				data.Equipment = equipmentType;
			else
				throw new DataParseError("Equipment");

			// FireMode
			if (Enum.TryParse<FireModeType>(dataTable["FireMode"].ToString(), true, out var fireModeType))
				data.FireMode = fireModeType;
			else
				throw new DataParseError("FireMode");

			// MagazineCapacity
			if (int.TryParse(dataTable["MagazineCapacity"].ToString(), out var magazineCapacity))
				data.MagazineCapacity = magazineCapacity;
			else
				throw new DataParseError("MagazineCapacity");

			// MaxAmmo
			if (int.TryParse(dataTable["MaxAmmo"].ToString(), out var maxAmmo))
				data.MaxAmmo = maxAmmo;
			else
				throw new DataParseError("MaxAmmo");

			// UseDelay
			if (float.TryParse(dataTable["UseDelay"].ToString(), out var useDelay))
				data.UseDelay = useDelay;
			else
				throw new DataParseError("UseDelay");

			// ReloadDelay
			if (float.TryParse(dataTable["ReloadDelay"].ToString(), out var reloadDelay))
				data.ReloadDelay = reloadDelay;
			else
				throw new DataParseError("ReloadDelay");

			// Recoil
			if (float.TryParse(dataTable["Recoil"].ToString(), out var recoil))
				data.Recoil = recoil;
			else
				throw new DataParseError("Recoil");

			// AccuracyIncrease
			if (float.TryParse(dataTable["AccuracyIncrease"].ToString(), out var accuracyIncrease))
				data.AccuracyIncrease = accuracyIncrease;
			else
				throw new DataParseError("AccuracyIncrease");

			// AccuracyRecovery
			if (float.TryParse(dataTable["AccuracyRecovery"].ToString(), out var accuracyRecovery))
				data.AccuracyRecovery = accuracyRecovery;
			else
				throw new DataParseError("AccuracyRecovery");

			// AccuracyRecovery
			if (float.TryParse(dataTable["MaxAccuracy"].ToString(), out var maxAccuracy))
				data.MaxAccuracy = maxAccuracy;
			else
				throw new DataParseError("MaxAccuracy");

			// AccuracyRecovery
			if (float.TryParse(dataTable["MinAccuracy"].ToString(), out var minAccuracy))
				data.MinAccuracy = minAccuracy;
			else
				throw new DataParseError("MinAccuracy");
			#endregion

			#region Weapon Info
			WeaponInfo weaponInfo = new WeaponInfo();

			weaponInfo.WeaponType = equipmentType;

			// EffectType
			if (Enum.TryParse<EffectType>(dataTable["EffectType"].ToString(), true, out var effectType))
				weaponInfo.EffectType = effectType;
			else
				throw new DataParseError("EffectType");

			// HitscanType
			if (Enum.TryParse<HitscanType>(dataTable["HitscanType"].ToString(), true, out var hitscanType))
				weaponInfo.HitscanType = hitscanType;
			else
				throw new DataParseError("HitscanType");

			// PenetrateCount
			if (int.TryParse(dataTable["PenetrateCount"].ToString(), out var penetrateCount))
				weaponInfo.PenetrateCount = (byte)penetrateCount;
			else
				throw new DataParseError("PenetrateCount");

			// MaxDistance
			if (float.TryParse(dataTable["MaxDistance"].ToString(), out var maxDistance))
				weaponInfo.MaxDistance = maxDistance;
			else
				throw new DataParseError("MaxDistance");

			// Damage
			if (int.TryParse(dataTable["Damage"].ToString(), out var damage))
				weaponInfo.Damage = (short)damage;
			else
				throw new DataParseError("Damage");

			// Speed
			if (float.TryParse(dataTable["Speed"].ToString(), out var speed))
				weaponInfo.Speed = speed;
			else
				throw new DataParseError("Speed");

			// Damage
			if (bool.TryParse(dataTable["AllowFriendlyFire"].ToString(), out var allowFriendlyFire))
				weaponInfo.AllowFriendlyFire = allowFriendlyFire;
			else
				throw new DataParseError("AllowFriendlyFire");
			#endregion

			data.WeaponInfo = weaponInfo;

			return data;
		}
	}

#if UNITY_EDITOR
	public void OnValidate()
	{
		BindNetworkObject(@"\Prefabs\NetworkObjects");
		BindMapPrefabs(@"\Prefabs\Maps\");
		BindEffectPrefabs(@"\Prefabs\Effects Sync\");
		BindHitscanPrefabs(@"\Prefabs\Hitscans\");
		BindProfileSprites(@"\Sprites\Character\Profile\");
		BindAmmoInfoSprites(@"\Sprites\GUI\AmmoInfo\");
		BindGuiPrefabs(@"\Prefabs\MonoGUI\");
	}

	public void BindNetworkObject(string relativePath)
	{
		// Bind network object
		NetworkObjectTable = new();

		var networkObjectPath = AssetLoader.DataPathOnEnvironment + relativePath;
		foreach (var netObject in AssetLoader.GetPrefabsFileFromPath(networkObjectPath))
		{
			var netComponent = netObject.GetComponent<NetworkObject>();
			if (!NetworkObjectTable.TryAddUniqueByKey(netComponent.Type, netObject))
			{
				Ulog.LogError(this, $"There is duplicated network type exist in {netObject.name}, " +
					$"Class : {netComponent.GetType().Name}");
				return;
			}
		}

		Ulog.Log(this, $"Bind NetworkObjectPrefabs! Count : {NetworkObjectTable.Count}");
	}

	public void BindMapPrefabs(string relativePath)
	{
		// Bind map prefabs
		MapTable = new();
		var mapPath = AssetLoader.DataPathOnEnvironment + relativePath;
		foreach (var mapObject in AssetLoader.GetPrefabsFileFromPath(mapPath, new List<string>() { "Map_" }))
		{
			var mapComponent = mapObject.GetComponent<MapHandler>();
			if (mapComponent == null)
			{
				continue;
			}

			if (!MapTable.TryAddUniqueByKey(mapComponent.Type, mapObject))
			{
				Ulog.LogError(this, $"There is duplicated map type exist in {mapObject.name}, " +
					$"Class : {mapComponent.GetType().Name}");
				return;
			}
		}

		Ulog.Log(this, $"Bind MapObjectPrefabs! Count : {MapTable.Count}");
	}

	public void BindGuiPrefabs(string relativePath)
	{
		// Bind GUI Prefabs
		GuiTable = new();
		var guiPath = AssetLoader.DataPathOnEnvironment + relativePath;
		GuiPrefabList = AssetLoader.GetPrefabsFileFromPath(guiPath, new List<string>() { "View_" });
		Ulog.Log(this, $"Bind GUI Prefabs! Count : {GuiPrefabList.Count}");
	}

	public void BindProfileSprites(string relativePath)
	{
		ProfileTable = new();
		var characterSprtiePath = AssetLoader.DataPathOnEnvironment + relativePath;
		var characterSpriteList = AssetLoader.GetAssetsFromPath<Sprite>(characterSprtiePath);

		int startIndex = "spr_profile_".Length;

		foreach (CharacterType c in Enum.GetValues(typeof(CharacterType)))
		{
			var match = c.ToString().ToLower();

			var sprite = characterSpriteList.Find((sprite) =>
			{
				if (sprite.name.Length <= startIndex)
				{
					return false;
				}

				var spriteName = sprite.name.ToLower().Substring(startIndex);
				return spriteName == match;
			});

			if (sprite != null)
			{
				if (!ProfileTable.TryAddUniqueByKey(c, sprite))
				{
					Ulog.LogError(this, $"There is duplicated item sprite exist in {sprite.name}, Type : {c}");
					return;
				}
				characterSpriteList.Remove(sprite);
			}
		}
		Ulog.Log(this, $"Bind Portrait sprites! Count : {ProfileTable.Count}");
	}

	public void BindAmmoInfoSprites(string relativePath)
	{
		AmmoInfoTable = new();
		var ammoInfoSprtiePath = AssetLoader.DataPathOnEnvironment + relativePath;
		var ammoInfoSpriteList = AssetLoader.GetAssetsFromPath<Sprite>(ammoInfoSprtiePath);

		int startIndex = "spr_ammoInfo_".Length;

		foreach (AmmoInfoType ammo in Enum.GetValues(typeof(AmmoInfoType)))
		{
			var match = ammo.ToString().ToLower();

			var sprite = ammoInfoSpriteList.Find((sprite) =>
			{
				if (sprite.name.Length <= startIndex)
				{
					return false;
				}

				var spriteName = sprite.name.ToLower().Substring(startIndex);
				return spriteName == match;
			});

			if (sprite != null)
			{
				if (!AmmoInfoTable.TryAddUniqueByKey(ammo, sprite))
				{
					Ulog.LogError(this, $"There is duplicated item sprite exist in {sprite.name}, Type : {ammo}");
					return;
				}
				ammoInfoSpriteList.Remove(sprite);
			}
		}
		Ulog.Log(this, $"Bind AmmoInfo sprites! Count : {ProfileTable.Count}");
	}

	public void BindEffectPrefabs(string relativePath)
	{
		// Bind GUI Prefabs
		EffectPrefabTable = new();
		var guiPath = AssetLoader.DataPathOnEnvironment + relativePath;
		var effects = AssetLoader.GetPrefabsFileFromPath(guiPath, new List<string>() { "Effect_" });

		foreach (EffectType e in Enum.GetValues(typeof(EffectType)))
		{
			var go = effects.Find((effectGo) =>
			{
				return effectGo.name == e.ToString();
			});

			if (go != null)
			{
				if (!EffectPrefabTable.TryAddUniqueByKey(e, go))
				{
					Ulog.LogError(this, $"There is duplicated effectd object exist! Type : {e}, Go Name : {go.name}");
					return;
				}

				effects.Remove(go);
			}
		}

		Ulog.Log(this, $"Bind effect Prefabs! Count : {EffectPrefabTable.Count}");
	}

	public void BindHitscanPrefabs(string relativePath)
	{
		// Bind GUI Prefabs
		HitscanPrefabTable = new();
		var guiPath = AssetLoader.DataPathOnEnvironment + relativePath;
		var hitscans = AssetLoader.GetPrefabsFileFromPath(guiPath, new List<string>() { "Hitscan_" });

		foreach (HitscanType e in Enum.GetValues(typeof(HitscanType)))
		{
			var go = hitscans.Find((prefab) =>
			{
				return prefab.name == e.ToString();
			});

			if (go != null)
			{
				// If there is no hitscan component
				var hitscanComponent = go.GetComponent<HitscanBase>();
				if (hitscanComponent == null)
				{
					continue;
				}

				if (!HitscanPrefabTable.TryAddUniqueByKey(e, go))
				{
					Ulog.LogError(this, $"There is duplicated {typeof(HitscanType).Name} object exist!" +
						$"Type : {e}, Go Name : {go.name}");
					return;
				}

				hitscans.Remove(go);
			}
		}

		Ulog.Log(this, $"Bind {typeof(HitscanType).Name} Prefabs! Count : {HitscanPrefabTable.Count}");
	}

	public void ParseItemTable(string dataPath)
	{

	}
#endif
}