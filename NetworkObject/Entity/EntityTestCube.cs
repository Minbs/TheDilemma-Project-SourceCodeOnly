using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using Utils;
using Sirenix.OdinInspector;

public class EntityTestCube : NetworkObject
{
	[ShowInInspector]
	public override NetObjectType Type => NetObjectType.Entity_TestCube;

	[SerializeField]
	[SyncVar(SyncType.UnreliableFixed)]
	public SyncField<NetVector3> Position = new();

	[Button]
	public void release()
	{
		this.Release();
	}

	public override void Client_OnStart()
	{
		Position.ResetDeserializeEvent();
		Position.OnDeserialized += onSyncPositionChanged;
	}

	private void onSyncPositionChanged(NetVector3 position)
	{
		transform.position = position;
	}

	public override void Server_OnBeforeNetworkTickUpdate()
	{
		Position.Data = transform.position;
	}
}
