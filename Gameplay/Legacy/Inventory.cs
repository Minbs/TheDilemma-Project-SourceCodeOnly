using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	/// <summary>인벤토리입니다.</summary>
	public class Inventory
	{
		public Int8Vector2 Capacity { get; private set; }
		public bool[,] CollisionMap { get; private set; }
		public int Width => Capacity.X;
		public int Height => Capacity.Y;

		private List<ItemBase> mItems;
		public int Count => mItems.Count;
		public ItemBase[] Items => mItems.ToArray();

		public Inventory(Int8Vector2 capacity)
		{
			Capacity = capacity;
			CollisionMap = new bool[capacity.Y, capacity.X];
			mItems = new List<ItemBase>();
		}

		/// <summary>아이템의 개수를 반환받습니다.</summary>
		/// <param name="type">아이템 타입입니다.</param>
		/// <returns>아이템의 총 개수입니다.</returns>
		public int GetItemCountBy(ItemType type)
		{
			int count = 0;

			foreach (var i in mItems)
			{
				if (i.Type == type)
				{
					count += i.Count;
				}
			}

			return count;
		}

		/// <summary>아이템의 묶음 개수를 반환받습니다.</summary>
		/// <param name="type">아이템 타입입니다.</param>
		/// <returns>해당 아이템의 묶음 개수입니다.</returns>
		public int GetItemBundleCountBy(ItemType type)
		{
			int count = 0;

			foreach (var i in mItems)
			{
				if (i.Type == type)
				{
					count++;
				}
			}

			return count;
		}

		public void Clear()
		{
			for (int i = mItems.Count - 1; i >= 0; i--)
			{
				TryRemove(mItems[i]);
			}
		}

		/// <summary>개수가 없는 아이템은 인벤토리 목록에서 제거합니다.</summary>
		/// <returns></returns>
		public void Refresh()
		{
			for (int i = Items.Length - 1; i >= 0; i--)
			{
				if (Items[i].Count <= 0)
				{
					TryRemove(Items[i]);
				}
			}
		}

		/// <summary>아이템을 추가합니다.</summary>
		/// <param name="item">추가할 아이템입니다.</param>
		public InventoryOperationResult TryAdd(in ItemBase item)
		{
			if (mItems.Contains(item))
			{
				return InventoryOperationResult.ItemAlreadyExist;
			}

			Int8Vector2 emptyPos;

			if (HasSpace(item.Transform.Size, out emptyPos))
			{
				stackItem(item, emptyPos);
				return InventoryOperationResult.Success;
			}
			else
			{
				item.Transform.Rotate();
				if (HasSpace(item.Transform.Size, out emptyPos))
				{
					stackItem(item, emptyPos);
					return InventoryOperationResult.Success;
				}
			}

			return InventoryOperationResult.ThereIsNoSpace;

			void stackItem(ItemBase item, Int8Vector2 emptyPos)
			{
				// Try stack item
				foreach (var i in mItems)
				{
					if (i.TryStack(item))
					{
						if (item.Count == 0)
						{
							return;
						}
					}
				}

				mItems.Add(item);
				item.Transform.MoveTo(emptyPos);
				fillCollisionMap(item.Transform, true);
				return;
			}
		}

		/// <summary>아이템을 제거합니다.</summary>
		/// <param name="item">제거할 아이템입니다.</param>
		public InventoryOperationResult TryRemove(ItemBase item)
		{
			if (mItems.Contains(item))
			{
				fillCollisionMap(item.Transform, false);
				mItems.Remove(item);
				return InventoryOperationResult.Success;
			}

			return InventoryOperationResult.ThereIsNoSuchItem;
		}

		/// <summary>
		/// 지정한 타입의 아이템을 개수만큼 제거합니다. 아이템 개수가 부족하면 false를 반환합니다.
		/// </summary>
		/// <param name="type">제거할 아이템입니다.</param>
		/// <param name="count">제거할 수량입니다.</param>
		/// <returns>제거할 아이템 개수가 충분하지 않으면 false를 반환합니다.</returns>
		public InventoryOperationResult TryRemoveBy(ItemType type, int count)
		{
			if (count > GetItemCountBy(type))
			{
				return InventoryOperationResult.LackOfItems;
			}

			for (int i = mItems.Count - 1; i >= 0; i--)
			{
				if (count <= 0)
				{
					return InventoryOperationResult.Success;
				}

				var item = mItems[i];

				if (item.Type != type)
				{
					continue;
				}

				if (item.Count <= count)
				{
					mItems.RemoveAt(i);
					count -= item.Count;
				}
				else
				{
					item.TryDiscount(count);
					return InventoryOperationResult.Success;
				}
			}

			return InventoryOperationResult.Success;
		}

		/// <summary>사용가능한 아이템을 Type으로 찾습니다.</summary>
		/// <param name="type">찾을 아이템 타입입니다.</param>
		/// <param name="useableItem">사용가능한 아이템입니다.</param>
		/// <returns>
		/// 아이템이 존재하지 않는다면 false를 반환합니다.
		/// 사용이 불가능한 아이템인 경우에도 false를 반환합니다.
		/// </returns>
		public InventoryOperationResult TryPopUseableItemBy(ItemType type, out UseableItem useableItem)
		{
			foreach (var i in mItems)
			{
				if (i.ItemType == type)
				{
					useableItem = i as UseableItem;
					if (useableItem != null)
					{
						return InventoryOperationResult.Success;
					}

					Ulog.LogWarning(this, $"Item type error! Type : {i.ItemType}");
				}
			}

			useableItem = null;
			return InventoryOperationResult.ThereIsNoSuchItem;
		}

		private void fillCollisionMap(ItemTransform itemTransform, bool isFill)
		{
			this.fillCollisionMap(itemTransform.Position, itemTransform.Size, isFill);
		}

		private void fillCollisionMap(Int8Vector2 position, Int8Vector2 size, bool isFill)
		{
			for (int y = position.Y; y < position.Y + size.Y; y++)
			{ 
				for (int x = position.X; x < position.X + size.X; x++)
				{
					CollisionMap[y, x] = isFill;
				}
			}
		}

		public bool HasSpace(Int8Vector2 size, out Int8Vector2 position)
		{
			for (int y = 0; y <= Height - size.Y; y++)
			{
				for (int x = 0; x <= Width - size.X; x++)
				{
					Int8Vector2 checkPos = (x, y);

					if (IsEmpty(checkPos, size))
					{
						position = checkPos;
						return true;
					}
				}
			}

			position = Int8Vector2.Zero;
			return false;
		}

		public bool IsEmpty(Int8Vector2 position, Int8Vector2 size)
		{
			for (int y = position.Y; y < position.Y + size.Y; y++)
			{
				for (int x = position.X; x < position.X + size.X; x++)
				{
					if (CollisionMap[y, x])
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
