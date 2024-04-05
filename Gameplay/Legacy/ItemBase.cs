using System;
using KaNet.Synchronizers;
using KaNet.Utils;
using Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public abstract class ItemBase : INetworkSerializable
	{
		/// <summary>아이템 타입입니다.</summary>
		public NetUInt16<ItemType> Type { get; protected set; }
		/// <summary>아이템의 위치를 나타내는 Transform입니다.</summary>
		public ItemTransform Transform { get; protected set; }
		/// <summary>아이템의 개수입니다.</summary>
		public NetUInt8 Count { get; protected set; }
		/// <summary>최대로 쌓을 수 있는 개수입니다.</summary>
		public NetUInt8 MaxStack { get; protected set; }
		/// <summary>아이템 사용에 걸리는 시간입니다.</summary>
		public NetFloat UseDelay { get; protected set; }

		#region Getter
		public ItemType ItemType => Type.GetEnum();
		public BaseItemType BaseType => Type.GetEnum().GetBaseType();
		#endregion

		public ItemBase
		(
			ItemType itemType,
			Int8Vector2 size,
			NetUInt8 count,
			NetUInt8 maxStack,
			NetFloat useDelay)
		{
			Type = itemType;
			Transform = new ItemTransform((0, 0), size, false);
			Count = count;
			MaxStack = maxStack;
			UseDelay = useDelay;
		}

		#region Network
		public virtual int GetSyncDataSize()
		{
			return Type.GetSyncDataSize() +
				Transform.GetSyncDataSize() +
				Count.GetSyncDataSize() +
				MaxStack.GetSyncDataSize() +
				UseDelay.GetSyncDataSize();
		}

		public virtual void SerializeTo(in NetPacketWriter writer)
		{
			Type.SerializeTo(writer);
			Transform.SerializeTo(writer);
			Count.SerializeTo(writer);
			MaxStack.SerializeTo(writer);
			UseDelay.SerializeTo(writer);
		}

		public virtual void DeserializeFrom(in NetPacketReader reader)
		{
			Type.DeserializeFrom(reader);
			Transform.DeserializeFrom(reader);
			Count.DeserializeFrom(reader);
			MaxStack.DeserializeFrom(reader);
			UseDelay.DeserializeFrom(reader);
		}
		#endregion

		/// <summary>아이템 개수를 수량만큼 감소시킵니다.</summary>
		/// <param name="count">감소시킬 수량</param>
		/// <returns>아이템 수량이 충분하지 않으면 false를 반환합니다.</returns>
		public bool TryDiscount(int count)
		{
			if (Count < count)
			{
				return false;
			}

			Count -= (byte)count;
			return true;
		}

		/// <summary>아이템 묶음을 쌓습니다.</summary>
		/// <param name="other">쌓을 아이템입니다. 쌓고나서 남은 아이템이 반환됩니다.</param>
		/// <returns>타입이 다르면 false를 반환합니다.</returns>
		public bool TryStack(in ItemBase other)
		{
			if (this.Type.Value != other.Type.Value)
			{
				return false;
			}

			Count += other.Count;

			if (Count <= MaxStack)
			{
				other.Count = 0;
			}
			else
			{
				other.Count = (byte)(Count - MaxStack);
				this.Count = MaxStack;
			}

			return true;
		}

		/// <summary>아이템을 사용합니다.</summary>
		/// <returns>사용 결과입니다.</returns>
		public abstract InventoryOperationResult TryUse(Inventory inventory);

		/// <summary>한 칸짜리 아이템 정의입니다.</summary>
		public static Int8Vector2 Size_OneCell => (1, 1);
	}
}
