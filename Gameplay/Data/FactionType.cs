using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Utils;

namespace Gameplay
{
	public enum FactionMatchType : byte
	{
		Neutral = 0,
		Alliance,
		Enemy,
	}

	/// <summary>해당 객체의 전체 소속 타입입니다.</summary>
	public enum BaseFactionType : byte
	{
		Neutral = 0,
		Human,
		Creature,
	}

	/// <summary>해당 객체의 소속 타입입니다.</summary>
	public enum FactionType : byte
	{
		// 중립
		Neutral = 0,

		Human,
		Human_A,
		Human_B,
		Human_C,

		Creature,
		Creature_A,
		Creature_B,
		Creature_C,
	}

	public static class FactionTypeExtension
	{
		private static GenericEnumTableUInt8<FactionType, BaseFactionType> mBaseFactionTypeTable = new()
		{
			{ FactionType.Human,   BaseFactionType.Human },
			{ FactionType.Creature,   BaseFactionType.Creature },
		};

		public static BaseFactionType GetBaseType(this FactionType type) => mBaseFactionTypeTable.GetBaseType(type);
		public static bool IsBaseType(this FactionType type, BaseFactionType baseType) => type.GetBaseType() == baseType;
		public static bool IsEnemy(this FactionType type, FactionType other)
		{
			var otherBaseType = other.GetBaseType();

			if (otherBaseType == BaseFactionType.Neutral)
			{
				return false;
			}

			return otherBaseType != type.GetBaseType();
		}
		public static bool IsAlliance(this FactionType type, FactionType other)
		{
			var otherBaseType = other.GetBaseType();

			if (otherBaseType == BaseFactionType.Neutral)
			{
				return false;
			}

			return otherBaseType == type.GetBaseType();
		}

		public static bool IsMatch(this FactionType type, FactionType other, FactionMatchType factionMatchType)
		{
			if (factionMatchType == FactionMatchType.Enemy)
			{
				return type.IsEnemy(other);
			}
			else if (factionMatchType == FactionMatchType.Alliance)
			{
				return type.IsAlliance(other);
			}
			else if (factionMatchType == FactionMatchType.Neutral)
			{
				return !type.IsEnemy(other) && !type.IsAlliance(other);
			}

			return false;
		}
	}
}
