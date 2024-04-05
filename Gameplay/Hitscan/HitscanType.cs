using Utils;

namespace Gameplay
{
	public enum BaseHitscanType
	{
		None = 0,

		Instant,
		Simulate,
		Synchronize,
	}

	public enum HitscanType : ushort
	{
		None = 0,

		/// <summary>발사 즉시 결과가 결정되는 Hitscan입니다. 클라이언트가 판정 권한을 갖습니다.</summary>
		Hitscan_Instant,
		Hitscan_Instant_Area,
		Hitscan_Instant_Laser,
		Hitscan_Instant_Point,

		/// <summary>시뮬레이션되는 Hitscan입니다. 클라이언트가 판정 권한을 갖습니다.</summary>
		Hitscan_Simulate,
		Hitscan_Simulate_Projectile,

		/// <summary>동기화되는 Hitscan입니다. 서버가 판정 권한을 갖습니다.</summary>
		Hitscan_Synchronize,
	}

	public static class HitscanTypeExtension
	{
		private static GenericEnumTableUInt16<HitscanType, BaseHitscanType> mBaseHitscanTypeTable = new()
		{
			{ HitscanType.Hitscan_Instant,   BaseHitscanType.Instant },
			{ HitscanType.Hitscan_Simulate,   BaseHitscanType.Simulate },
			{ HitscanType.Hitscan_Synchronize,   BaseHitscanType.Synchronize },
		};

		public static BaseHitscanType GetBaseType(this HitscanType type) => mBaseHitscanTypeTable.GetBaseType(type);
		public static bool IsBaseType(this HitscanType type, BaseHitscanType baseType) => type.GetBaseType() == baseType;
	}
}
