using KaNet.Synchronizers;
using KaNet.Utils;

namespace Gameplay
{
	/// <summary>발사자의 정보 구조체입니다.</summary>
	public struct AttackerInfo : INetworkSerializable
	{
		/// <summary>발사자의 Session ID입니다. AI가 발사한 경우 의미가 없습니다.</summary>
		public NetSessionID AttackerID;
		/// <summary>발사한 객체의 Network Object ID 입니다.</summary>
		public NetObjectID ObjectID;
		/// <summary>발사한 객체의 Object Type입니다.</summary>
		public NetUInt16<NetObjectType> ObjectType;
		/// <summary>발사한 객체의 소속 타입입니다.</summary>
		public NetUInt8<FactionType> Faction;

		public bool IsFromPlayer => ObjectType.GetEnum().IsPlayerType();

		public AttackerInfo(EntityBase attacker)
		{
			AttackerID = attacker.OwnerID;
			ObjectID = attacker.ID;
			ObjectType = attacker.Type;
			Faction = attacker.Faction;
		}

		public AttackerInfo
		(
			NetSessionID sessionID,
			NetObjectID objectID,
			NetObjectType objectType,
			FactionType faction
		)
		{
			AttackerID = sessionID;
			ObjectID = objectID;
			ObjectType = objectType;
			Faction = faction;
		}

		#region Network
		public int GetSyncDataSize()
		{
			return AttackerID.GetSyncDataSize() +
				ObjectID.GetSyncDataSize() +
				ObjectType.GetSyncDataSize() +
				Faction.GetSyncDataSize();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			AttackerID.SerializeTo(writer);
			ObjectID.SerializeTo(writer);
			ObjectType.SerializeTo(writer);
			Faction.SerializeTo(writer);
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			AttackerID.DeserializeFrom(reader);
			ObjectID.DeserializeFrom(reader);
			ObjectType.DeserializeFrom(reader);
			Faction.DeserializeFrom(reader);
		}
		#endregion
	}
}
