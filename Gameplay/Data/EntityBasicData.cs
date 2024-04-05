using System;
using Gameplay;
using KaNet.Synchronizers;
using KaNet.Utils;

public struct EntityBasicData : INetworkSerializable
{
	public NetUInt8<FactionType> Faction;
	public NetInt32 HP;
	public NetFloat MoveSpeed;

	public EntityBasicData
	(
		FactionType faction,
		int hp,
		float moveSpeed
	)
	{
		Faction = faction;
		HP = hp;
		MoveSpeed = moveSpeed;
	}

	public int GetSyncDataSize()
	{
		return
			Faction.GetSyncDataSize() +
			HP.GetSyncDataSize() +
			MoveSpeed.GetSyncDataSize();
	}

	public void SerializeTo(in NetPacketWriter writer)
	{
		Faction.SerializeTo(writer);
		HP.SerializeTo(writer);
		MoveSpeed.SerializeTo(writer);
	}

	public void DeserializeFrom(in NetPacketReader reader)
	{
		Faction.DeserializeFrom(reader);
		HP.DeserializeFrom(reader);
		MoveSpeed.DeserializeFrom(reader);
	}
}
