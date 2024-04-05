using Utils;

namespace Gameplay
{
	public enum BaseEquipmentType : byte
	{
		None = 0,
		/// <summary>주무기 입니다.</summary>
		Primary,
		/// <summary>보조무기 입니다.</summary>
		Secondary,
		/// <summary>보조 장비입니다.</summary>
		Auxilliary,
		/// <summary>크리쳐의 장비입니다.</summary>
		Creature,
	}

	public enum EquipmentType : byte
	{
		None = 0,

		/// <summary>주무기 입니다.</summary>
		Primary = 10,

		PumpActionShotgun,
		LightMachineGun,
		SniperRifle,
		LaserRifle,

		/// <summary>보조무기 입니다.</summary>
		Secondary = 50,

		SubmachineGun,
		PistolGrenadeLauncher,
		HeavyPistol,
		TaserGun,

		/// <summary>보조 장비입니다.</summary>
		Auxilliary = 100,

		StunGrenade,
		Steampack,
		FreezeGrenade,
		HealDrone,

		/// <summary>크리쳐의 장비입니다.</summary>
		Creature = 150,
	}

	public static class EquipmentTypeExtension
	{
		private static GenericEnumTableUInt8<EquipmentType, BaseEquipmentType> mBaseTypeTable = new()
		{
			{ EquipmentType.Primary,   BaseEquipmentType.Primary },
			{ EquipmentType.Secondary,   BaseEquipmentType.Secondary },
			{ EquipmentType.Auxilliary,   BaseEquipmentType.Auxilliary },
			{ EquipmentType.Creature,   BaseEquipmentType.Creature },
		};

		public static BaseEquipmentType GetBaseType(this EquipmentType type) => mBaseTypeTable.GetBaseType(type);

		public static bool IsBaseType(this EquipmentType type, BaseEquipmentType baseType) => type.GetBaseType() == baseType;

		public static bool IsWeaponType(this EquipmentType type)
		{
			var baseType = type.GetBaseType();
			return baseType == BaseEquipmentType.Primary ||
				baseType == BaseEquipmentType.Secondary;
		}

		public static bool IsAuxilliary(this EquipmentType type)
		{
			return type.IsBaseType(BaseEquipmentType.Auxilliary);
		}
	}
}
