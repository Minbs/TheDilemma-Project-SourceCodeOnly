using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;

using Utils;

public class KaNetMonoNetworkObject : NetworkObject
{
	public void Start()
	{
		NetworkObjectPrebinder.PrebindByReflection(this.GetType(), this);
	}

	[ShowInInspector]
	public override NetObjectType Type => NetObjectType.None;

	//public override NetworkObjectType Type => throw new System.NotImplementedException();

	[SyncVar]
	public readonly SyncField<NetInt32> SomeValue = new(12345);

	[RpcCall]
	public readonly RpcCaller<NetInt32, NetBool> RPC_DoSomething = new();
	public void DoSomething(NetInt32 arg1, NetBool arg2)
	{
		int v = arg1 + 10;
		bool b = arg2;
	}

	public void SomeAction()
	{
		RPC_DoSomething.Invoke(500, false);
	}

	#region Server Side

	/// <summary>서버측에서 생성되었을 때 호출됩니다. Start함수와 유사합니다.</summary>
	public override void Server_OnStart()
	{

	}

	/// <summary>서버측에서 제거되었을 때 호출됩니다. OnDestroy함수와 유사합니다.</summary>
	public override void Server_OnDestroy()
	{

	}

	/// <summary>서버측에서 Update될 때 호출됩니다. Update함수와 유사합니다.</summary>
	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	/// <summary>서버측에서 FixedUpdate될 때 호출됩니다. FixedUpdate함수와 유사합니다.</summary>
	public override void Server_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	#endregion

	#region Client Side

	/// <summary>클라이언트측에서 생성되었을 때 호출됩니다. Start함수와 유사합니다.</summary>
	public override void Client_OnStart()
	{

	}

	/// <summary>클라이언트측에서 제거되었을 때 호출됩니다. OnDestroy함수와 유사합니다.</summary>
	public override void Client_OnDestroy()
	{

	}

	/// <summary>클라이언트측에서 Update될 때 호출됩니다. Update함수와 유사합니다.</summary>
	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	/// <summary>클라이언트측에서 FixedUpdate될 때 호출됩니다. FixedUpdate함수와 유사합니다.</summary>
	public override void Client_OnFixedUpdate(in DeltaTimeInfo deltaTimeInfo)
	{

	}

	#endregion
}
