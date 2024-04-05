using System.Runtime.InteropServices;
using System;
using KaNet.Synchronizers;
using KaNet.Utils;
using Utils;

namespace Gameplay.Legacy
{
	[Obsolete]
	/// <summary>2차원 정수 좌표계 구조체입니다. 데카르트 좌표계를 사용합니다.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Int8Vector2 : INetworkSerializable
	{
		public NetInt8 X;
		public NetInt8 Y;

		public readonly static Int8Vector2 Zero = new Int8Vector2(0, 0);
		public readonly static Int8Vector2 One = new Int8Vector2(1, 1);
		public readonly static Int8Vector2 Right = new Int8Vector2(1, 0);
		public readonly static Int8Vector2 Left = new Int8Vector2(-1, 0);
		public readonly static Int8Vector2 Up = new Int8Vector2(0, 1);
		public readonly static Int8Vector2 Down = new Int8Vector2(0, -1);
		public readonly static Int8Vector2 RightUp = new Int8Vector2(1, 1);
		public readonly static Int8Vector2 RightDown = new Int8Vector2(1, -1);
		public readonly static Int8Vector2 LeftUp = new Int8Vector2(-1, 1);
		public readonly static Int8Vector2 LeftDown = new Int8Vector2(-1, -1);

		public static implicit operator Int8Vector2((int x, int y) value) => new Int8Vector2(value.x, value.y);

		/// <summary>2차원 정수 좌표 구조체를 생성합니다.</summary>
		/// <param name="x">X 좌표</param>
		/// <param name="y">Y 좌표</param>
		public Int8Vector2(int x, int y)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
		}

		#region Network

		public int GetSyncDataSize()
		{
			return X.GetSyncDataSize() + Y.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			X.SerializeTo(writer);
			Y.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			X.DeserializeFrom(reader);
			Y.DeserializeFrom(reader);
		}

		#endregion

		#region Operation

		public static Int8Vector2 operator +(Int8Vector2 lhs, Int8Vector2 rhs)
		{
			return new Int8Vector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		public static Int8Vector2 operator -(Int8Vector2 lhs, Int8Vector2 rhs)
		{
			return new Int8Vector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		public static Int8Vector2 operator *(Int8Vector2 lhs, int rhs)
		{
			return new Int8Vector2(lhs.X * rhs, lhs.Y * rhs);
		}

		public static Int8Vector2 operator *(int lhs, Int8Vector2 rhs)
		{
			return new Int8Vector2(rhs.X * lhs, rhs.Y * lhs);
		}

		public static Int8Vector2 operator /(Int8Vector2 lhs, int rhs)
		{
			return new Int8Vector2(lhs.X / rhs, lhs.Y / rhs);
		}

		public static bool operator ==(Int8Vector2 lhs, Int8Vector2 rhs)
		{
			return (lhs.X == rhs.X && lhs.Y == rhs.Y);
		}

		public static bool operator !=(Int8Vector2 lhs, Int8Vector2 rhs)
		{
			return (lhs.X != rhs.X || lhs.Y != rhs.Y);
		}

		public override bool Equals(object obj)
		{
			return obj is Int8Vector2 && this == (Int8Vector2)obj;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(this.X, this.Y).GetHashCode();
		}

		#endregion

		public override string ToString()
		{
			return $"({X},{Y})";
		}
	}
}
