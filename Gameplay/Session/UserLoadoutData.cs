using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public enum InventoryOperationResult
	{
		None,
		Success,
		ThereIsNoSpace,
		ThereIsNoSuchItem,
		WrongSlotIndex,
		LackOfItems,
		NotUseableItem,
		ItemAlreadyExist,
	}

	[Obsolete("더 이상 사용하지 않음")]
	public class UserLoadoutData
	{
		public Inventory Inventory { get; private set; }
		public WeaponBase[] WeaponSlot = new WeaponBase[GlobalGameplayData.WEAPON_SLOT_COUNT];
		public ItemType[] ItemQuickSlot = new ItemType[GlobalGameplayData.ITEM_SLOT_COUNT];

		public int WeaponSelector { get; private set; }

		public UserLoadoutData
		(
			Int8Vector2 inventorySize
		)
		{
			Inventory = new Inventory(inventorySize);
		}

		public void Clear()
		{
			WeaponSelector = 0;

			Inventory.Clear();

			for (int i = 0; i < WeaponSlot.Length; i++)
			{
				WeaponSlot[i] = null;
			}

			for (int i = 0; i < ItemQuickSlot.Length; i++)
			{
				ItemQuickSlot[i] = ItemType.None;
			}
		}

		/// <summary>무기 셀렉터를 갱신합니다.</summary>
		public void RefreshWeaponSelector()
		{
			if (WeaponSlot[WeaponSelector] == null)
			{
				if (WeaponSelector == 0)
				{
					WeaponSelector = 1;
				}
			}
		}

		public void SwapWeapon()
		{
		}

		/// <summary>무기를 사용합니다.</summary>
		/// <param name="index">사용할 무기의 인덱스입니다.</param>
		/// <param name="weapon">사용한 무기입니다.</param>
		/// <returns>인벤토리 Operation 결과입니다.</returns>
		private InventoryOperationResult tryUseWeapon(int index, out WeaponBase weapon)
		{
			weapon = null;
			if (index >= WeaponSlot.Length || index < 0)
			{
				return InventoryOperationResult.WrongSlotIndex;
			}

			weapon = WeaponSlot[index];
			var result = weapon.TryUse(Inventory);
			return result;
		}

		/// <summary>퀵 슬롯 아이템을 사용합니다.</summary>
		/// <param name="index">사용할 퀵 슬롯의 인덱스입니다.</param>
		/// <param name="usedItem">사용한 아이템입니다.</param>
		/// <returns>개수가 충분하지 않거나 유효하지 않은 Index인 경우 false를 반환합니다.</returns>
		private InventoryOperationResult tryUseQuickItem(int index, out UseableItem usedItem)
		{
			if (index >= ItemQuickSlot.Length || index < 0)
			{
				usedItem = null;
				return InventoryOperationResult.WrongSlotIndex;
			}

			var curItemType = ItemQuickSlot[index];

			var findResult = Inventory.TryPopUseableItemBy(curItemType, out usedItem);
			if (findResult != InventoryOperationResult.Success)
			{
				// There is no item in the inventory
				return findResult;
			}

			var useResult = usedItem.TryUse(Inventory);
			if (useResult != InventoryOperationResult.Success)
			{
				return useResult;
			}

			if (Inventory.GetItemCountBy(curItemType) <= 0)
			{
				// Clear quick slot if there is no item in the inventory
				ItemQuickSlot[index] = ItemType.None;
			}

			return InventoryOperationResult.Success;
		}

		/// <summary>아이템을 큇슬롯에 바인딩합니다.</summary>
		/// <param name="index">바인딩할 슬롯입니다.</param>
		/// <param name="bindItem">바인딩할 아이템입니다.</param>
		public void BindQuickSlot(int index, UseableItem bindItem)
		{
			ItemQuickSlot[index] = bindItem.Type;
		}

		/// <summary>무기 슬롯을 바인딩합니다.</summary>
		/// <param name="index">바인딩할 무기 인덱스입니다.</param>
		/// <param name="bindWeapon">바인딩할 무기입니다.</param>
		public InventoryOperationResult TryBindWeaponSlot(int index, WeaponBase bindWeapon)
		{
			// Pop weapon
			var popResult = Inventory.TryRemove(bindWeapon);
			if (popResult != InventoryOperationResult.Success)
			{
				return popResult;
			}

			// Push slot weapon
			var previousWeapon = WeaponSlot[index];
			if (previousWeapon != null)
			{
				var pushResult = Inventory.TryAdd(previousWeapon);
				if (pushResult != InventoryOperationResult.Success)
				{
					return pushResult;
				}
			}

			WeaponSlot[index] = bindWeapon;
			return InventoryOperationResult.Success;
		}
	}
}
