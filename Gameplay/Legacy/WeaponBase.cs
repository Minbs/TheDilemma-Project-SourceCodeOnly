using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using KaNet.Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public class WeaponBase : ItemBase
	{
		/// <summary>현재 남은 탄약입니다.</summary>
		public NetUInt16 AmmoLeft { get; private set; }
		public WeaponItemInfo WeaponItemInfo { get; private set; }

		public WeaponBase
		(
			ItemType itemType, 
			Int8Vector2 size, 
			NetFloat fireDelay,
			WeaponItemInfo weaponInfo
		)
			: base(itemType, size, 1, 1, fireDelay)
		{
			WeaponItemInfo = weaponInfo;
			AmmoLeft = WeaponItemInfo.Capacity;
		}

		public override int GetSyncDataSize()
		{
			return base.GetSyncDataSize() +
				AmmoLeft.GetSyncDataSize() +
				WeaponItemInfo.GetSyncDataSize();
		}

		public override void SerializeTo(in NetPacketWriter writer)
		{
			base.SerializeTo(writer);
			AmmoLeft.SerializeTo(writer);
			WeaponItemInfo.SerializeTo(writer);
		}

		public override void DeserializeFrom(in NetPacketReader reader)
		{
			base.DeserializeFrom(reader);
			AmmoLeft.DeserializeFrom(reader);
			WeaponItemInfo.DeserializeFrom(reader);
		}

		public override InventoryOperationResult TryUse(Inventory inventory)
		{
			//var ammoType = WeaponItemInfo.WeaponInfo.AmmoType;
			//return inventory.TryRemoveBy(ammoType, WeaponItemInfo.ConsumCount);
			return InventoryOperationResult.None;
		}
	}
}
