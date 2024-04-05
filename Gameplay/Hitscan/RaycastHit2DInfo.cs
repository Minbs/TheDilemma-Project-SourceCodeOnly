using KaNet.Synchronizers;
using KaNet.Utils;
using UnityEngine;

namespace Gameplay
{
	public struct RaycastHit2DInfo : INetworkSerializable
	{
		/// <summary>피격당한 대상의 ID 입니다.</summary>
		public NetObjectID Target;
		/// <summary>피격 지점입니다.</summary>
		public NetVector2 HitPosition;
		/// <summary>피격 노말입니다.</summary>
		public NetVector2 HitNormal;
		/// <summary>피격당한 부위의 데미지 곱입니다.</summary>
		public NetFloat HitboxDamageMultiply;

		public RaycastHit2DInfo
		(
			NetObjectID target,
			NetVector2 hitPosition,
			NetVector2 hitNormal,
			NetFloat hitboxDamageMultiply
		)
		{
			Target = target;
			HitPosition = hitPosition;
			HitNormal = hitNormal;
			HitboxDamageMultiply = hitboxDamageMultiply;
		}

		public RaycastHit2DInfo(Hitbox hitbox, RaycastHit2D raycastHit)
		{
			Target = hitbox.Entity.ID;
			HitboxDamageMultiply = hitbox.DamageMultiplier;
			HitPosition = raycastHit.point;
			HitNormal = raycastHit.normal;
		}

		public RaycastHit2DInfo(Hitbox hitbox, NetVector2 point)
		{
			Target = hitbox.Entity.ID;
			HitboxDamageMultiply = hitbox.DamageMultiplier;
			HitPosition = point;
			HitNormal = Vector2.right;
		}

		public int GetSyncDataSize()
		{
			return Target.GetSyncDataSize() +
				HitboxDamageMultiply.GetSyncDataSize() +
				HitPosition.GetSyncDataSize() +
				HitNormal.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			Target.SerializeTo(writer);
			HitboxDamageMultiply.SerializeTo(writer);
			HitPosition.SerializeTo(writer);
			HitNormal.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Target.DeserializeFrom(reader);
			HitboxDamageMultiply.DeserializeFrom(reader);
			HitPosition.DeserializeFrom(reader);
			HitNormal.DeserializeFrom(reader);
		}
	}
}
