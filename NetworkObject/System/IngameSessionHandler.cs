using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Session;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;
using Sirenix.OdinInspector;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Utils;
using Utils.Service;

public struct IngameSessionInfo : INetworkSerializable
{
	public NetSessionID ID;
	public NetBool IsReadyToPlay;
	public NetUInt8<CharacterType> Character;
	public NetUInt64 SteamID;

	public Friend Friend => new Friend(SteamID.Value);
	public string Name => Friend.Name;

	public IngameSessionInfo(NetSessionInfo sessionInfo)
	{
		ID = sessionInfo.ID;
		IsReadyToPlay = false;
		Character =	CharacterType.None;
		SteamID = sessionInfo.SteamID.Value;
	}

	public int GetSyncDataSize()
	{
		return ID.GetSyncDataSize() +
			IsReadyToPlay.GetSyncDataSize() +
			Character.GetSyncDataSize() +
			SteamID.GetSyncDataSize();
	}

	public void SerializeTo(in NetPacketWriter writer)
	{
		ID.SerializeTo(writer);
		IsReadyToPlay.SerializeTo(writer);
		Character.SerializeTo(writer);
		SteamID.SerializeTo(writer);
	}

	public void DeserializeFrom(in NetPacketReader reader)
	{
		ID.DeserializeFrom(reader);
		IsReadyToPlay.DeserializeFrom(reader);
		Character.DeserializeFrom(reader);
		SteamID.DeserializeFrom(reader);
	}

	public override string ToString()
	{
		return $"{Name} : {ID}";
	}
}

public class IngameSessionHandler : NetworkObject
{
	public override NetObjectType Type => NetObjectType.System_IngameSessionHandler;

	/// <summary>현재 클라이언트 기준에서 접속이 확인된 세션 테이블입니다.</summary>
	private BidirectionalMap<NetSessionID, NetSessionInfo> mCurrentSessionTable = new();

	private GameplayManager mGameplayManager;

	/// <summary>서버에서 관리되는 접속된 인원에 대한 정보입니다.</summary>
	[ShowInInspector][SyncVar]
	private readonly SyncList<IngameSessionInfo> mIngameSessions = new();
	public IReadOnlyList<IngameSessionInfo> IngameSessions => mIngameSessions.DataList;

	public event Action<IngameSessionHandler> OnSessionChanged;

	public event Action<NetSessionID> OnSessionConnected;
	public event Action<NetSessionID> OnSessionDisconnected;

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	public override void Common_OnStart()
	{
		if (IsServerSide)
		{
			mIngameSessions.OnChanged += () => OnSessionChanged?.Invoke(this);
		}
		else
		{
			mIngameSessions.OnDeserialized += () => OnSessionChanged?.Invoke(this);
		}
	}
	
	public override void Server_OnStart()
	{
		var readySessions = ObjectManager.ReadySessionInfoList;
		foreach (var s in readySessions)
		{
			onSessionConnected(s);
		}

		ObjectManager.OnReadySessionConnected += onSessionConnected;
		ObjectManager.OnSessionDisconnected += onSessionDisconnected;

		// 서버는 항상 준비된 상태이다.
		OnSelectCharacter(CharacterType.Police);
		OnRequestReady(true);
	}

	public override void Server_OnDestroy()
	{
		ObjectManager.OnReadySessionConnected -= onSessionConnected;
		ObjectManager.OnSessionDisconnected -= onSessionDisconnected;
	}

	private void onSessionConnected(NetSessionInfo sessionInfo)
	{
		if (IsServerSide)
		{
			if (!mIngameSessions.TryFind((s) => s.ID == sessionInfo.ID, out var session))
			{
				mIngameSessions.Add(new IngameSessionInfo(sessionInfo));
				RPC_Server_OnSessionConnected.Invoke(sessionInfo.ID);
			}
			else
			{
				Ulog.LogError(this, $"OnSessionConnected Error! Current session already connected! : {sessionInfo}");
			}
		}

		if (mCurrentSessionTable.TryAdd(sessionInfo.ID, sessionInfo))
		{
			Ulog.Log(this, $"Connected : {sessionInfo}");
		}
		else
		{
			Ulog.LogError(this, $"OnSessionConnected Error! Current session already connected! : {sessionInfo}");
		}
	}

	private void onSessionDisconnected(NetSessionInfo sessionInfo)
	{
		if (IsServerSide)
		{
			mIngameSessions.Remove((s) => s.ID == sessionInfo.ID);
			RPC_Server_OnSessionDisconnected.Invoke(sessionInfo.ID);
		}

		if (mCurrentSessionTable.TryRemove(sessionInfo))
		{
			Ulog.Log(this, $"Disconnected : {sessionInfo}");
		}
		else
		{
			Ulog.LogError(this, $"OnSessionDisconnect Error! There is no connected session! : {sessionInfo}");
		}
	}

	[RpcCall]
	private RpcCaller<NetSessionID> RPC_Server_OnSessionConnected = new();
	private void Server_OnSessionConnected(NetSessionID sessionID)
	{
		OnSessionConnected?.Invoke(sessionID);
	}

	[RpcCall]
	private RpcCaller<NetSessionID> RPC_Server_OnSessionDisconnected = new();
	private void Server_OnSessionDisconnected(NetSessionID sessionID)
	{
		OnSessionDisconnected?.Invoke(sessionID);
	}

	#region Operation

	public bool HasSession(NetSessionID id)
	{
		foreach (var s in mIngameSessions)
		{
			if (s.ID == id)
			{
				return true;
			}
		}

		return false;
	}

	public bool TryGetMySessionInfo(out IngameSessionInfo sessionInfo)
	{
		return TryGetSessionInfoByID(ClientID, out sessionInfo);
	}

	public bool TryGetSessionInfoByID(NetSessionID id, out IngameSessionInfo sessionInfo)
	{
		foreach (var s in mIngameSessions)
		{
			if (s.ID == id)
			{
				sessionInfo = s;
				return true;
			}
		}

		sessionInfo = new IngameSessionInfo();
		return false;
	}

	public bool TryGetSessionIndex(NetSessionID id, out int index)
	{
		for (int i = 0; i < mIngameSessions.Count; i++)
		{
			if (mIngameSessions[i].ID == id)
			{
				index = i;
				return true;
			}
		}
		
		index = -1;
		return false;
	}

	#endregion

	#region Lobby Operation

	/// <summary>현재 클라이언트가 준비중인 상태인지 여부를 반환합니다.</summary>
	public bool IsClientReady()
	{
		if (mIngameSessions.TryFind((s) => s.ID == ClientID, out var data))
		{
			return data.IsReadyToPlay;
		}
		
		return false;
	}

	public bool TryGetCurrentLobby(out Lobby currentLobby)
	{
		return GlobalServiceLocator
			.NetworkManageService
			.GetServiceOrNull()
			.TryGetCurrentLobby(out currentLobby);
	}

	/// <summary>모든 플레이어가 준비되고 캐릭터를 선택한 상태인지 검사합니다.</summary>
	/// <returns></returns>
	public bool Server_AreAllPlayerReadyToPlay()
	{
		foreach (var s in mIngameSessions)
		{
			if (!s.IsReadyToPlay)
			{
				return false;
			}

			if (s.Character.GetEnum() == CharacterType.None)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>캐릭터를 선택합니다.</summary>
	/// <param name="selectedCharacter">None타입이면 선택하지 않은것으로 간주합니다.</param>
	public void OnSelectCharacter(CharacterType selectedCharacter = CharacterType.None)
	{
		RPC_Client_RequestSelectCharacter.Invoke(ClientID, selectedCharacter);
	}

	[RpcCall(Authority: SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetUInt8<CharacterType>>
		RPC_Client_RequestSelectCharacter = new();
	private void Client_RequestSelectCharacter(NetSessionID sender, NetUInt8<CharacterType> selectedType)
	{
		if (TryGetSessionIndex(sender, out int index))
		{
			var session = mIngameSessions[index];

			// Check is unselect
			if (session.Character.GetEnum() == selectedType)
			{
				session.Character =	CharacterType.None;
				mIngameSessions[index] = session;
				return;
			}

			// Check is unselected character
			foreach (var s in mIngameSessions)
			{
				if (s.Character.GetEnum() == selectedType)
				{
					return;
				}
			}

			session.Character = selectedType;
			mIngameSessions[index] = session;
		}
		else
		{
			Ulog.LogError(this, $"There is no such session. Session ID : {sender}");
			return;
		}
	}

	/// <summary>로비에서 준비상태를 설정합니다.</summary>
	/// <param name="isReady">true이면 준비, false이면 준비 해제 및 취소입니다.</param>
	public void OnRequestReady(bool isReady)
	{
		RPC_Client_RequestReady.Invoke(ClientID, isReady);
	}

	[RpcCall(Authority: SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetBool> RPC_Client_RequestReady = new();
	private void Client_RequestReady(NetSessionID sender, NetBool isReady)
	{
		if (TryGetSessionIndex(sender, out int index))
		{
			var session = mIngameSessions[index];

			if (session.Character.GetEnum() == CharacterType.None)
			{
				// TODO : Cannot ready callback
				return;
			}

			session.IsReadyToPlay = isReady;
			mIngameSessions[index] = session;
		}
		else
		{
			Ulog.LogError(this, $"There is no such session. Session ID : {sender}");
			return;
		}
	}

	/// <summary>자신이 호스트인경우 게임을 시작합니다.</summary>
	public void Server_OnStartGame()
	{
		if (!IsServerSide)
		{
			Ulog.LogError(this, $"Server_OnStartGame failed! You are not server!");
			return;
		}

		if (!Server_AreAllPlayerReadyToPlay())
		{
			// TODO : Send System Message to host
			return;
		}

		// TODO : Start game
		mGameplayManager.Server_StartGame(MapType.map_underground_stage_1);
	}

	#endregion
}