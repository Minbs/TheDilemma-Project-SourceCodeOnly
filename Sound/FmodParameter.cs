using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using KaNet.Utils;

public struct SoundParameterInfo : INetworkSerializable
{
	public NetString Name;
	public NetFloat Value;

	public SoundParameterInfo(SoundParameter soundParameter)
	{
		Name = soundParameter.Name;
		Value = soundParameter.Value;
	}

	public SoundParameterInfo(NetString name, NetFloat value)
	{
		Name = name;
		Value = value;
	}

	public SoundParameter GetSoundParameter()
	{
		return new SoundParameter(Name, Value);
	}

	public int GetSyncDataSize()
	{
		return Name.GetSyncDataSize() + Value.GetSyncDataSize();
	}

	public void SerializeTo(in NetPacketWriter writer)
	{
		Name.SerializeTo(writer);
		Value.SerializeTo(writer);
	}

	public void DeserializeFrom(in NetPacketReader reader)
	{
		Name.DeserializeFrom(reader);
		Value.DeserializeFrom(reader);
	}
}

public class SoundParameter
{
	public SoundParameter(string name, float value)
	{
		Name = name;
		Value = value;
	}

	public string Name { get; set; }
	public float Value { get; set; }
}
