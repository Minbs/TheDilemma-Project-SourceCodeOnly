using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Gameplay;
using UnityEngine.UIElements;

public class SoundHandler : NetworkObject
{
	/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
	public override NetObjectType Type => NetObjectType.System_SoundHandler;

	private GameplayManager mGameplayManager;

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	#region Normal Effect

	/// <summary>동기화되는 Sound를 재생합니다.</summary>
	public void PlaySoundSync
	(
		NetUInt16<SoundType> type,
		NetVector2 position,
		SoundParameterInfo soundParam
	)
	{
		playSound(type, position, soundParam);
		RPC_Server_RequestPlaySound.Invoke(ClientID, type, position, soundParam);
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetUInt16<SoundType>, NetVector2, SoundParameterInfo>
		RPC_Server_RequestPlaySound = new();
	private void Server_RequestPlaySound
	(
		NetSessionID caller,
		NetUInt16<SoundType> type,
		NetVector2 position,
		SoundParameterInfo soundParam
	)
	{
		var sendTo = ObjectManager.GetNetSessionIDsExcept(caller);
		RPC_Client_PlaySound.Invoke(caller, type, position, soundParam, sendTo);
	}

	[RpcCall(SyncType.ReliableInstant, SyncAuthority.ServerOnly)]
	private readonly RpcCaller<NetSessionID, NetUInt16<SoundType>, NetVector2, SoundParameterInfo>
		RPC_Client_PlaySound = new();
	private void Client_PlaySound
	(
		NetSessionID caller,
		NetUInt16<SoundType> type,
		NetVector2 position,
		SoundParameterInfo soundParam
	)
	{
		playSound(type, position, soundParam);
	}

	private void playSound
	(
		NetUInt16<SoundType> type,
		NetVector2 position,
		SoundParameterInfo soundParam
	)
	{
		GlobalServiceLocator.SoundService.GetServiceOrNull()
			.Play(type, position.Value, soundParam.GetSoundParameter());
	}

	#endregion
}

