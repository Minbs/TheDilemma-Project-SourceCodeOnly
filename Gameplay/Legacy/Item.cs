using System;
using KaNet.Synchronizers;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	/// <summary>특별한 기능이 없는 일반 아이템입니다.</summary>
	public class Item : ItemBase
	{
		public Item(ItemType itemType, NetUInt8 count, NetUInt8 maxStack)
			: base(itemType, ItemBase.Size_OneCell, count, maxStack, 0) { }

		public override InventoryOperationResult TryUse(Inventory inventory)
		{
			return InventoryOperationResult.NotUseableItem;
		}
	}
}
