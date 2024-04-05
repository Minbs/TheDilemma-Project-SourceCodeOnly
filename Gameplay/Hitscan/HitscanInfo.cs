using System.Collections.Generic;
using KaNet.Synchronizers;
using KaNet.Utils;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
	/// <summary>발사된 Hitscan의 정보입니다.</summary>
	public struct HitscanInfo : INetworkSerializable
	{
		/// <summary>판정 권한을 가진 Session ID입니다.</summary>
		public NetSessionID AuthorityID;
		/// <summary>공격자 정보입니다.</summary>
		public AttackerInfo Attacker;
		/// <summary>발사된 무기의 정보입니다.</summary>
		public WeaponInfo WeaponInfo;
		/// <summary>피격자 정보입니다.</summary>
		public NetList<RaycastHit2DInfo> HitList;
		/// <summary>Hitscan의 시작 위치입니다.</summary>
		public NetVector2 Start;
		/// <summary>Hitscan의 방향입니다. Point 공격인 경우 Point 지점이 됩니다.</summary>
		public NetVector2 Direction;

		/// <summary>공격자 입장에서 Hitscan 정보를 생성합니다.</summary>
		public HitscanInfo
		(
			NetSessionID authorityID,
			EntityBase attacker, 
			WeaponInfo weapon,
			NetVector2 start,
			NetVector2 direction
		)
		{
			AuthorityID = authorityID;
			Attacker = new AttackerInfo(attacker);
			WeaponInfo = weapon;
			HitList = new();
			Start = start;
			Direction = direction;
		}

		public HitscanInfo(HitscanInfo hitscanInfo, IList<RaycastHit2DInfo> hitList)
		{
			this.AuthorityID = hitscanInfo.AuthorityID;
			this.Attacker = hitscanInfo.Attacker;
			this.WeaponInfo = hitscanInfo.WeaponInfo;
			this.HitList = new NetList<RaycastHit2DInfo>(hitList);
			this.Start = hitscanInfo.Start;
			this.Direction = hitscanInfo.Direction;
		}

		public void BindHitList(List<RaycastHit2DInfo> hitList)
		{
			HitList = new NetList<RaycastHit2DInfo>(hitList);
		}

		#region Network
		public int GetSyncDataSize()
		{
			return AuthorityID.GetSyncDataSize() +
				Attacker.GetSyncDataSize() +
				WeaponInfo.GetSyncDataSize() +
				HitList.GetSyncDataSize() +
				Start.GetSyncDataSize() + 
				Direction.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			AuthorityID.SerializeTo(writer);
			Attacker.SerializeTo(writer);
			WeaponInfo.SerializeTo(writer);
			HitList.SerializeTo(writer);
			Start.SerializeTo(writer);
			Direction.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			AuthorityID.DeserializeFrom(reader);
			Attacker.DeserializeFrom(reader);
			WeaponInfo.DeserializeFrom(reader);
			HitList.DeserializeFrom(reader);
			Start.DeserializeFrom(reader);
			Direction.DeserializeFrom(reader);
		}
		#endregion
	}
}
