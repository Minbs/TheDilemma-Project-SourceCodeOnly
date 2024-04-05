using System;
using Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public enum BaseItemType : ushort
	{
		None = 0,
		Ammo,
		Weapon,
		Equipment,
	}

	[Obsolete("더 이상 사용하지 않음")]
	public enum ItemType : ushort
	{
		None = 0,

		/// <summary>총알 타입입니다.</summary>
		Ammo = 29000,
		Ammo_Pistol_9mm, // 권총탄
		Ammo_Rifle_556, // 소총탄
		Ammo_Rifle_127, // 대구경탄
		Ammo_Shotgun_Shell, // 샷건탄
		Ammo_Energy_Shell, // 에너지 셸

		/// <summary>무기 타입입니다.</summary>
		Weapon = 30000,
		Weapon_Pistol_1,
		Weapon_Submachine_gun_1,
		Weapon_Rifle_1,
		Weapon_Shotgun_1,

		/// <summary>장비 타입입니다.</summary>
		Equipment = 35000,
		Equipment_BodyArmor_1,
	}

	[Obsolete("더 이상 사용하지 않음")]
	public static class ItemTypeExtension
	{
		private static GenericEnumTableUInt16<ItemType, BaseItemType> mBaseTypeTable = new()
			{
				{ ItemType.Ammo,   BaseItemType.Ammo },
				{ ItemType.Weapon,   BaseItemType.Weapon },
				{ ItemType.Equipment,   BaseItemType.Equipment },
			};

		public static BaseItemType GetBaseType(this ItemType type)
			=> mBaseTypeTable.GetBaseType(type);

		public static bool IsBaseType(this ItemType type, BaseItemType baseType)
			=> type.GetBaseType() == baseType;

		/// <summary>총알 아이템의 사격 타입을 반환받습니다.</summary>
		/// <returns>총알 아이템이 아닌 경우 false를 반환합니다.</returns>
		public static FireType GetFireType(this ItemType type)
		{
			if (!type.IsBaseType(BaseItemType.Ammo))
			{
				return FireType.None;
			}

			if (type <= ItemType.Ammo_Shotgun_Shell)
			{
				return FireType.Bullet;
			}
			else if (type <= ItemType.Ammo_Energy_Shell)
			{
				return FireType.Energy;
			}

			return FireType.None;
		}
	}
}
