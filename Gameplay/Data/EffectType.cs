using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Gameplay
{
	public enum BaseEffectType
	{
		None = 0,
		Effect_BulletMark = 10,
	}

	public enum EffectType : ushort
	{
		None = 0,

		Effect_Test = 10,
		Effect_BulletMark = 10000,
		Effect_BulletMark_Laser,
		Effect_BulletMark_HeavyPistol,
	}

	public static class EffectTypeExtension
	{
		private static GenericEnumTableUInt16<EffectType, BaseEffectType> mBaseFactionTypeTable = new()
		{
			{ EffectType.Effect_BulletMark,   BaseEffectType.Effect_BulletMark },
		};

		public static BaseEffectType GetBaseType(this EffectType type) => mBaseFactionTypeTable.GetBaseType(type);
		public static bool IsBaseType(this EffectType type, BaseEffectType baseType) => type.GetBaseType() == baseType;
	}
}
