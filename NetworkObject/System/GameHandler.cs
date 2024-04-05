using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using MonoGUI;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

public class GameHandler : NetworkObject
{
	public override NetObjectType Type => NetObjectType.System_GameHandler;

	[SerializeField] private GameplayManager mGameplayManager;

	// Timestamp Check
	private float mTsCheckInterval = 60.0f;
	private float mTsCheckDelay = 0;

	private NetTimestamp mServerTimestamp = 0;
	private Stopwatch mServerTsInterval = new Stopwatch();
	public NetTimestamp ServerTimestamp
	{
		get { return mServerTimestamp + mServerTsInterval.ElapsedMilliseconds; }
	}

	[SyncVar] public readonly SyncFieldByOrder<NetFloat> GameSpeed = new(1.0F);
	[SyncVar] public SyncField<NetUInt8<GameStateType>> CurrentGameState = new();

	public override void Common_OnStart()
	{
		GameSpeed.Data = 1.0F;

		this.ObjectManager.TryBindNetworkObjectAsType(this);

		mGameplayManager.Initialize(this);
	}

	#region Server Side
	public override void Server_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		mTsCheckDelay += deltaTimeInfo.ScaledDeltaTime;
		if (mTsCheckDelay > mTsCheckInterval)
		{
			mTsCheckDelay = 0;
			RPC_Client_setServerTick.Invoke(ObjectManager.CurrentTimestamp);
		}
	}

	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			GameSpeed.Data = 0.25f;
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			GameSpeed.Data = 1f;
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			GameSpeed.Data = 2f;
		}
	}

	#endregion

	#region Client Side

	public override void Client_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		ObjectManager.SetGlobalTimescale(GameSpeed.Data);
	}

	private RpcCaller<NetString> RPC_client_ChangeMap = new();
	private void client_ChangeMap(NetString mapName)
	{

	}

	[RpcCall(SyncType.UnreliableInstant)]
	private RpcCaller<NetTimestamp> RPC_Client_setServerTick = new();
	private void Client_setServerTick(NetTimestamp serverTick)
	{
		mServerTimestamp = serverTick;
		mServerTsInterval.Restart();
	}

	#endregion

	public void Disconnect()
	{
		var networkManager = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();
		networkManager.Disconnect();
	}

	public void Disconnect(string disconnectResone)
	{
		var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();
		var dialog = globalGui.ShowSystemDialog
		(
			NetOperationType.OnDisconnected,
			NetOperationType.OnDisconnected.GetTitle(),
			disconnectResone,
			(DialogResult.OK, true)
		);

		Disconnect();
	}
}

