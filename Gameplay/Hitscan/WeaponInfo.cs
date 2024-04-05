using KaNet.Synchronizers;
using KaNet.Utils;

namespace Gameplay
{
	/// <summary>발사된 무기의 정보입니다.</summary>
	public struct WeaponInfo : INetworkSerializable
	{
		/// <summary>무기의 타입입니다.</summary>
		public NetUInt8<EquipmentType> WeaponType;
		/// <summary>이펙트 타입입니다.</summary>
		public NetUInt16<EffectType> EffectType;
		/// <summary>히트스켄 타입입니다.</summary>
		public NetUInt16<HitscanType> HitscanType;

		/// <summary>관통 횟수입니다.</summary>
		public NetUInt8 PenetrateCount;
		/// <summary>최대 발사 거리입니다.</summary>
		public NetFloat MaxDistance;
		/// <summary>무기의 데미지입니다.</summary>
		public NetInt16 Damage;
		/// <summary>발사한 공격의 속도입니다.</summary>
		public NetFloat Speed;
		/// <summary>아군 오사 허용 여부입니다.</summary>
		public NetBool AllowFriendlyFire;

		public WeaponInfo
		(
			EquipmentType weaponType,
			EffectType effectType,
			HitscanType hitscanType,
			NetUInt8 penetrateCount,
			NetFloat maxDistance,
			NetInt16 damage,
			NetFloat speed,
			NetBool allowFriendlyFire
		)
		{
			WeaponType = weaponType;
			EffectType = effectType;
			HitscanType = hitscanType;

			PenetrateCount = penetrateCount;
			MaxDistance = maxDistance;
			Damage = damage;
			Speed = speed;
			AllowFriendlyFire = allowFriendlyFire;
		}

		#region Network
		public int GetSyncDataSize()
		{
			return
				WeaponType.GetSyncDataSize() +
				//AmmoType.GetSyncDataSize() +
				EffectType.GetSyncDataSize() +
				HitscanType.GetSyncDataSize() +

				PenetrateCount.GetSyncDataSize() +
				MaxDistance.GetSyncDataSize() +
				Damage.GetSyncDataSize() +
				Speed.GetSyncDataSize() +
				AllowFriendlyFire.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			WeaponType.SerializeTo(writer);
			//AmmoType.SerializeTo(writer);
			EffectType.SerializeTo(writer);
			HitscanType.SerializeTo(writer);

			PenetrateCount.SerializeTo(writer);
			MaxDistance.SerializeTo(writer);
			Damage.SerializeTo(writer);
			Speed.SerializeTo(writer);
			AllowFriendlyFire.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			WeaponType.DeserializeFrom(reader);
			//AmmoType.DeserializeFrom(reader);
			EffectType.DeserializeFrom(reader);
			HitscanType.DeserializeFrom(reader);

			PenetrateCount.DeserializeFrom(reader);
			MaxDistance.DeserializeFrom(reader);
			Damage.DeserializeFrom(reader);
			Speed.DeserializeFrom(reader);
			AllowFriendlyFire.DeserializeFrom(reader);
		}
		#endregion
	}
}
