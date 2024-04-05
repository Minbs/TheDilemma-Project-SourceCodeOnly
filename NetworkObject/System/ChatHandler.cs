using System;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using UnityEngine;

public class ChatHandler : NetworkObject
{
	public override NetObjectType Type => NetObjectType.System_ChatHandler;

	private GameplayManager mGameplayManager;

	//public IReadOnlyCollection<string> MessageQueue => mMessageQueue;
	public event Action<string> OnReceivedMessage;

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	#region User Send Message

	public void SendChatMessageToServer(string message)
	{
		RPC_Server_RelayMessage.Invoke(ClientID, message);
	}

	[RpcCall(Authority: SyncAuthority.None)]
	private readonly RpcCaller<NetSessionID, NetString> RPC_Server_RelayMessage = new();
	private void Server_RelayMessage(NetSessionID sender, NetString message)
	{
		if (mGameplayManager.IngameSessionHandler.TryGetSessionInfoByID(sender, out var sessionInfo))
		{
			string receivedMessage = $"{sessionInfo.Name} : {message}";
			RPC_Client_ReceiveMessage.Invoke(sender, receivedMessage);
		}
	}

	[RpcCall]
	private readonly RpcCaller<NetSessionID, NetString> RPC_Client_ReceiveMessage = new();
	private void Client_ReceiveMessage(NetSessionID sender, NetString message)
	{
		OnReceivedMessage?.Invoke(message);
	}

	#endregion

	#region	System Message

	public void Server_SendSystemMessageTo(NetSessionID to, NetString netString)
	{
		RPC_Client_ReceiveMessage.Invoke(ClientID, netString, to);
	}

	#endregion
}
