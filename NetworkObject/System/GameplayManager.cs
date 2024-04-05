using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Session;
using KaNet.Synchronizers;
using Utils.Service;

using UnityEngine;
using MonoGUI;
using Sirenix.OdinInspector;
using KaNet.Synchronizers.Prebinder;
using KaNet;
using Utils;
using System.Collections;

public class GameplayManager : NetworkObject
{
	public override NetObjectType Type => NetObjectType.System_GameplayManager;

	// DI
	private GameHandler mGameHandler;

	// Mono Initial System
	[Title("Initial System")]
	[field: SerializeField] public PlayerCamera PlayerCamera { get; private set; }

	[field : SerializeField] public IngameSessionHandler IngameSessionHandler { get; private set; }
	[field : SerializeField] public SystemEventDispatcher SystemEventDispatcher { get; private set; }
	[field : SerializeField] public DataHandler DataHandler { get; private set; }
	[field : SerializeField] public HitscanHandler HitscanHandler { get; private set; }
	[field : SerializeField] public EffectHandler EffectHandler { get; private set; }
	[field : SerializeField] public ChatHandler ChatHandler { get; private set; }
	[field : SerializeField] public EventManager EventManager { get; private set; }
	[field : SerializeField] public SoundHandler SoundHandler { get; private set; }

	// GUI System
	[Title("GUI System")]
	[field: SerializeField] public InGameGUISystem InGameGUISystem { get; private set; }

	// Game System
	public EntityService EntityService { get; private set; }
	private PlayerInputService mPlayerInputService;

	private MapController mMapController = new();

	[SyncVar] public SyncField<NetUInt8<GameStateType>> GameState = new();

	public void Initialize(GameHandler gameHandler)
	{
		mGameHandler = gameHandler;

		// Mono Initial System
		IngameSessionHandler.InitializeByManager(this);
		SystemEventDispatcher.InitializeByManager(this);
		DataHandler.InitializeByManager(this);
		HitscanHandler.InitializeByManager(this);
		EffectHandler.InitializeByManager(this);
		ChatHandler.InitializeByManager(this);
		EventManager.InitializeByManager(this);
		SoundHandler.InitializeByManager(this);

		// GameSystem
		EntityService = new EntityService(this);
		mPlayerInputService = new PlayerInputService(EntityService);
		mPlayerInputService.InitializeByManager(this);

		//InGameGUISystem
		InGameGUISystem.InitializeByManager(this);
	}

	public override void Common_OnStart()
	{
		this.ObjectManager.TryBindNetworkObjectAsType(this);

		EntityService.OnRegistered();
		mPlayerInputService.OnRegistered();
	}

	public override void Server_OnAfterStart()
	{
		GameState.Data = GameStateType.Lobby;
		InGameGUISystem.SwitchToLobby(IngameSessionHandler);
	}

	/// <summary>클라이언트로써 서버로 초기 데이터 요청을 보냅니다.</summary>
	public override void Client_OnAfterStart()
	{
		RPC_Client_RequestGameState.Invoke(ClientID);
	}

	public override void Common_OnDestroy()
	{
		mPlayerInputService.OnUnregistered();
		EntityService.OnUnregistered();
	}

	#region Debug

	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			this.Server_ChangeMap(GetNextMap());
		}
	}

	#endregion

	#region Game State

	/// <summary>
	/// 클라이언트로써 서버에 초기 게임 상태를 요청합니다.
	/// 요청은 다른 RPC로 돌아옵니다.
	/// </summary>
	[RpcCall(Authority: SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID> RPC_Client_RequestGameState = new();
	private void Client_RequestGameState(NetSessionID sender)
	{
		if (IsServerSide)
		{
			RPC_Server_ResponseGameState.Invoke(GameState.Data, sender);
		}
	}

	[RpcCall]
	/// <summary>서버의 초기 게임 상태 응답입니다. 로비가 아닌 경우 접속을 종료합니다.</summary>
	private readonly RpcCaller<NetUInt8<GameStateType>> RPC_Server_ResponseGameState = new();
	private void Server_ResponseGameState(NetUInt8<GameStateType> currentGameState)
	{
		if (currentGameState.GetEnum() != GameStateType.Lobby)
		{
			mGameHandler.Disconnect($"해당 로비는 이미 플레이중입니다.");
			return;
		}

		InGameGUISystem.SwitchToLobby(IngameSessionHandler);
	}
	
	/// <summary>서버로서 게임을 시작합니다.</summary>
	public void Server_StartGame(MapType startMap)
	{
		if (!IsServerSide)
		{
			return;
		}

		// Setup initial data
		this.DataHandler.OnGameStart();

		// Change server side map
		Server_ChangeMap(startMap);
	}

	public void Server_ChangeMap(MapType changeTo)
	{
		GameState.Data = GameStateType.ServerLoading;

		// Start clients change map
		var broadcast = ObjectManager.GetNetSessionIDsExcept(ClientID);
		RPC_Server_RequestChangeMap.Invoke(changeTo, broadcast);

		GlobalServiceLocator
			.GlobalGuiService
			.GetServiceOrNull()
			.OpenGUILoading(() =>
		{
			Ulog.Log(this, $"Change map to {changeTo}");
			mMapController.ChangeMap(changeTo, this);

			// Spawn Players
			var spawnPoints = mMapController.CurrentMap.PlayerSpawnPoints;
			var connectedSessions = IngameSessionHandler.IngameSessions;

			foreach (var session in connectedSessions)
			{
				int count = spawnPoints.Count;
				int spawnIndex = session.ID % count;
				var position = spawnPoints[spawnIndex].SpawnPosition;

				DataHandler.TryGetPlayerCharacterData(session.Character, out var data);
				EntityService.KillEntity(false);
				EntityService.Server_SpawnPlayer(session.ID, position, data);
			}

			StartCoroutine(serverChangeMapRoutine());
		});
	}

	private IEnumerator serverChangeMapRoutine()
	{
		yield return new WaitForSeconds(GlobalGameplayData.MapChangeFadeoutSec);

		// Synchronize map
		RPC_Client_StartGame.Invoke();
	}

	/// <summary>클라이언트의 게임을 시작시킵니다.</summary>
	[RpcCall]
	private RpcCaller RPC_Client_StartGame = new(); 
	private void Client_StartGame()
	{
		InGameGUISystem.SwitchToAlive(this);
		InGameGUISystem.ShowSystemMessage(MessageType.Normal, $"Start Game");

		GlobalServiceLocator
			.GlobalGuiService
			.GetServiceOrNull()
			.CloseGUILoading(() =>
		{
			if (IsServerSide)
			{
				GameState.Data = GameStateType.Stage;
			}
		});
	}

	#endregion

	#region Map

	[RpcCall]
	private readonly RpcCaller<NetUInt8<MapType>> RPC_Server_RequestChangeMap = new();
	private void Server_RequestChangeMap(NetUInt8<MapType> changeTo)
	{
		GlobalServiceLocator
			.GlobalGuiService
			.GetServiceOrNull()
			.OpenGUILoading(() =>
		{
			Ulog.Log(this, $"Change map to {changeTo.GetEnum()}");
			mMapController.ChangeMap(changeTo, this);
		});
	}

	public MapType CurrentMap => mMapController.CurrentMap.Type;

	/// <summary>다음 맵을 반환합니다.</summary>
	/// <returns></returns>
	public MapType GetNextMap()
	{
		return CurrentMap + 1;
	}

	#endregion

	#region Input
	public void ProcessHealSelf()
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessHealSelf();
	}

	public void ProcessInteract()
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessInteract();
	}

	public void ProcessReload()
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessReload();
	}

	public void ProcessEquipmentSwapInput(int index)
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessEquipmentSwapInput(index);
	}

	public void ProcessMovementInput(NetVector2 moveInput)
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessMovementInput(moveInput);
	}

	public void ProcessMousePressed(bool isLeft)
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessMousePressed(isLeft);
	}

	public void ProcessMousePressing(bool isLeft)
	{
		if (GameState.Data == GameStateType.Stage)
			EntityService.PlayerEntityService.ProcessMousePressing(isLeft);
	}
	#endregion
}
