using System;
using KaNet.Synchronizers;
using KaNet.Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public struct WeaponItemInfo : INetworkSerializable
	{
		/// <summary>무기의 히트스캔 정보입니다.</summary>
		public WeaponInfo WeaponInfo { get; set; }

		/// <summary>총기의 탄창 용량입니다.</summary>
		public NetUInt16 Capacity { get; set; }
		/// <summary>1회 사용당 소모되는 횟수입니다.</summary>
		public NetUInt8 ConsumCount { get; set; }
		/// <summary>반동입니다.</summary>
		public NetFloat Recoil { get; set; }
		/// <summary>반동 제어률입니다.</summary>
		public NetFloat RecoilHandling { get; set; }

		//public FireType FireType => WeaponInfo.AmmoType.GetEnum().GetFireType();

		public WeaponItemInfo
		(
			WeaponInfo weaponInfo,
			NetUInt16 capacity,
			NetUInt8 consumCount,
			NetFloat recoil,
			NetFloat recoilHandling
		)
		{
			WeaponInfo = weaponInfo;
			Capacity = capacity;
			ConsumCount = consumCount;
			Recoil = recoil;
			RecoilHandling = recoilHandling;
		}

		public int GetSyncDataSize()
		{
			return
				WeaponInfo.GetSyncDataSize() +

				Capacity.GetSyncDataSize() +
				ConsumCount.GetSyncDataSize() +
				Recoil.GetSyncDataSize() +
				RecoilHandling.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			WeaponInfo.SerializeTo(writer);

			Capacity.SerializeTo(writer);
			ConsumCount.SerializeTo(writer);
			Recoil.SerializeTo(writer);
			RecoilHandling.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			WeaponInfo.DeserializeFrom(reader);

			Capacity.DeserializeFrom(reader);
			ConsumCount.DeserializeFrom(reader);
			Recoil.DeserializeFrom(reader);
			RecoilHandling.DeserializeFrom(reader);
		}
	}
}
