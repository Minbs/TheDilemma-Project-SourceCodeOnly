using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using KaNet.Session;
using UnityEngine.UIElements;

public class EntityTestSpawner : NetworkObject
{
	/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
	public override NetObjectType Type => NetObjectType.Entity_TestSpanwer;

	public string DebugMessage = "안녕?";

	[SyncVar(SyncType.UnreliableInstant)]
	private SyncField<NetInt32> mRandomValue = new();

	[RpcCall(Authority: SyncAuthority.ServerOnly)]
	public RpcCaller<NetString, NetInt32, NetFloat> RPC_PrintDebug = new();
	private void PrintDebug(NetString message, NetInt32 testValue_1, NetFloat testValue_2)
	{
		Ulog.Log(this, $"{message}, {testValue_1}, {testValue_2}");
	}

	[RpcCall(Authority: SyncAuthority.None)]
	private RpcCaller<NetUInt64, NetVector3, NetString> RPC_PrintDebug_2 = new();
	private void PrintDebug_2(NetUInt64 abc, NetVector3 vec, NetString str)
	{
		Ulog.Log(this, $"{abc}, {vec}, {str}");
	}

	[Button]
	private void createCube(NetInt32 repeatCount)
	{
		for (int i = 0; i < 1; i++)
		{
			Vector3 randPos = new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20));

			ObjectManager.CreateNetworkObjectAsServer<EntityTestCube>
			(
				NetObjectType.Entity_TestCube,
				0,
				randPos,
				Quaternion.identity
			);
		}
	}

	#region Server Side

	private List<Entity_PlayerController> mPlayerList = new List<Entity_PlayerController>();

	//private void createPlayer(NetSessionInfo session)
	//{
	//	var player = mObjectManager.CreateNetworkObjectAsServer<EntityPlayer>
	//	(
	//		NetObjectType.Entity_Player,
	//		session.ID,
	//		new Vector3(0, 0, -2),
	//		Quaternion.identity
	//	);

	//	mPlayerList.Add(player);
	//}

	private void removePlayer(NetSessionInfo session)
	{
		var player = mPlayerList.Find((netObj) => netObj.OwnerID == session.ID);
		player.Release();
	}

	public override void Server_OnStart()
	{
		//mObjectManager.OnSessionConnected += createPlayer;
		var myInfo = ObjectManager.ClientNetSessionInfo;
		//createPlayer(myInfo);

		for (int i = 0; i < 300; i++)
		{
			Vector3 randPos = new Vector3
			(
				Random.Range(-3.0f, 3.0f), 
				0, 
				Random.Range(-4.0f, -2.0f)
			);
			var player = ObjectManager.CreateNetworkObjectAsServer<Entity_PlayerController>
			(
				NetObjectType.Entity_Player,
				new NetSessionID((byte)i),
				randPos,
				Quaternion.identity
			);
			mPlayerList.Add(player);
		}
	}

	[Button]
	public void CreateNewPlayer()
	{
		var myInfo = ObjectManager.ClientNetSessionInfo;

		Vector3 randPos = new Vector3
		(
			Random.Range(-3.0f, 3.0f),
			0,
			Random.Range(-4.0f, -2.0f)
		);
		var player = ObjectManager.CreateNetworkObjectAsServer<Entity_PlayerController>
		(
			NetObjectType.Entity_Player,
			new NetSessionID((byte)0),
			randPos,
			Quaternion.identity
		);
		mPlayerList.Add(player);
	}

	public override void Server_OnDestroy()
	{
		//mObjectManager.OnSessionConnected -= createPlayer;
	}

	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			for (int i = 0; i < 1000; i++)
			{
				RPC_PrintDebug.Invoke(DebugMessage, i, 111.111f);
				RPC_PrintDebug_2.Invoke(13513553189, new NetVector3(new Vector3(i, i * 2, i * 3)), "카카");
			}
		}

		mRandomValue.Data = Random.Range(0, 10000);
	}

	public override void Server_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
	}

	#endregion

	#region Client Side

	public override void Client_OnStart()
	{
		
	}

	public override void Client_OnDestroy()
	{
		
	}

	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		//Ulog.Log(this, $"Test value {mRandomValue.Data}");
	}

	public override void Client_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		
	}

	#endregion
}
