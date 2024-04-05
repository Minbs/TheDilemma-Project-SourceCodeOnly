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

/// <summary>서버가 제어하고 동기화되는 Hitscan입니다.</summary>
public abstract class NetHitscanBase : NetworkObject
{
	protected GameplayManager mGameplayManager;

	// Synchronize Position
	[SyncVar(SyncType.UnreliableFixed, SyncAuthority.ServerOnly)]
	private readonly SyncFieldByOrder<NetVector3> mPositionByServer = new();

	[SyncVar(SyncType.UnreliableFixed, SyncAuthority.ServerOnly)]
	private readonly SyncFieldByOrder<NetFloat> mRotationByServer = new();

	public override void Common_OnStart()
	{
		mPositionByServer.ResetTimestamp();
		mPositionByServer.ResetDeserializeEvent();

		mRotationByServer.ResetTimestamp();
		mRotationByServer.ResetDeserializeEvent();
	}

	public override void Server_OnStart()
	{
		base.Server_OnStart();
		mPositionByServer.Data = transform.position;
		mRotationByServer.Data = transform.rotation.z;
	}

	public override void Client_OnStart()
	{
		base.Client_OnStart();
		transform.position = mPositionByServer.Data;
		transform.rotation = Quaternion.Euler(0, 0, mRotationByServer.Data);
	}

	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (IsServerSide)
		{
			mPositionByServer.Data = transform.position;
			mRotationByServer.Data = transform.rotation.eulerAngles.z;
		}
		else
		{
			transform.position = Vector3.Lerp
			(
				transform.position,
				mPositionByServer.Data,
				KaNetGlobal.NETWORK_INTERPOLATION_VALUE * deltaTimeInfo.DeltaTime
			);

			float rotationZ = Mathf.Lerp
			(
				transform.rotation.eulerAngles.z,
				mRotationByServer.Data,
				KaNetGlobal.NETWORK_INTERPOLATION_VALUE * deltaTimeInfo.DeltaTime
			);
			transform.rotation = Quaternion.Euler(0, 0, rotationZ);
		}
	}

	public void InitializeBy(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	/// <summary>자신의 위치에 Effect를 생성합니다.</summary>
	public void CreateEffect(EffectType effectType)
	{
		mGameplayManager.EffectHandler.CreateSyncEffect
		(
			effectType,
			transform.position.ToVector2(),
			transform.rotation
		);
	}

	public void PlaySound(SoundType soundType)
	{

	}
}
