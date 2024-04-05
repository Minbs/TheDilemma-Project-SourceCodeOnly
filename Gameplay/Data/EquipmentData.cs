using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using KaNet.Utils;
using Sirenix.OdinInspector;

namespace Gameplay
{
	[Serializable]
	public struct EquipmentData : INetworkSerializable
	{
		public NetUInt8<EquipmentType> Equipment;
		public NetUInt8<FireModeType> FireMode;
		public NetInt32 MagazineCapacity;
		public NetInt32 MaxAmmo;
		public NetFloat UseDelay;
		public NetFloat ReloadDelay;
		public NetFloat Recoil;
		public NetFloat MinAccuracy;
		public NetFloat MaxAccuracy;
		public NetFloat AccuracyRecovery;
		public NetFloat AccuracyIncrease;
		public WeaponInfo WeaponInfo;

		public int GetSyncDataSize()
		{
			return
				Equipment.GetSyncDataSize() +
				FireMode.GetSyncDataSize() +
				MagazineCapacity.GetSyncDataSize() +
				MaxAmmo.GetSyncDataSize() +
				UseDelay.GetSyncDataSize() +
				ReloadDelay.GetSyncDataSize() +
				Recoil.GetSyncDataSize() +
				MinAccuracy.GetSyncDataSize() +
				MaxAccuracy.GetSyncDataSize() +
				AccuracyRecovery.GetSyncDataSize() +
				AccuracyIncrease.GetSyncDataSize() +
				WeaponInfo.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			Equipment.SerializeTo(writer);
			FireMode.SerializeTo(writer);
			MagazineCapacity.SerializeTo(writer);
			MaxAmmo.SerializeTo(writer);
			UseDelay.SerializeTo(writer);
			ReloadDelay.SerializeTo(writer);
			Recoil.SerializeTo(writer);
			MinAccuracy.SerializeTo(writer);
			MaxAccuracy.SerializeTo(writer);
			AccuracyRecovery.SerializeTo(writer);
			AccuracyIncrease.SerializeTo(writer);
			WeaponInfo.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Equipment.DeserializeFrom(reader);
			FireMode.DeserializeFrom(reader);
			MagazineCapacity.DeserializeFrom(reader);
			MaxAmmo.DeserializeFrom(reader);
			UseDelay.DeserializeFrom(reader);
			ReloadDelay.DeserializeFrom(reader);
			Recoil.DeserializeFrom(reader);
			MinAccuracy.DeserializeFrom(reader);
			MaxAccuracy.DeserializeFrom(reader);
			AccuracyRecovery.DeserializeFrom(reader);
			AccuracyIncrease.DeserializeFrom(reader);
			WeaponInfo.DeserializeFrom(reader);
		}
	}
}
