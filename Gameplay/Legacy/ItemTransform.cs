using System.Runtime.InteropServices;
using System;
using KaNet.Synchronizers;
using KaNet.Utils;
using Utils;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public class ItemTransform : INetworkSerializable
	{
		public Int8Vector2 Position { get; private set; }
		public Int8Vector2 Size => (IsRotated) ? (mSize.Y, mSize.X) : mSize;
		private Int8Vector2 mSize;
		public NetBool IsRotated { get; private set; }

		public ItemTransform(Int8Vector2 position, Int8Vector2 size, NetBool isRotated)
		{
			Position = position;
			mSize = size;
			IsRotated = isRotated;
		}

		#region Network

		public int GetSyncDataSize()
		{
			return Position.GetSyncDataSize() + mSize.GetSyncDataSize() + IsRotated.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			Position.SerializeTo(writer);
			mSize.SerializeTo(writer);
			IsRotated.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Position.DeserializeFrom(reader);
			mSize.DeserializeFrom(reader);
			IsRotated.DeserializeFrom(reader);
		}

		#endregion

		public void MoveTo(Int8Vector2 position)
		{
			Position = position;
		}

		public void SetRotation(bool isRotate)
		{
			IsRotated = isRotate;
		}

		public void Rotate()
		{
			IsRotated = !IsRotated;
		}

		public bool IsCollideWith(in ItemTransform other)
		{
			Int8Vector2 thisSize = IsRotated ? new Int8Vector2(mSize.Y, mSize.X) : mSize;
			Int8Vector2 otherSize = other.IsRotated ? new Int8Vector2(other.mSize.Y, other.mSize.X) : other.mSize;

			thisSize -= (1, 1);
			otherSize -= (1, 1);

			return !((Position.X + thisSize.X < other.Position.X) ||
				(Position.Y + thisSize.Y < other.Position.Y) ||
				(Position.X > other.Position.X + otherSize.X) ||
				(Position.Y > other.Position.Y + otherSize.Y));
		}
	}
}
