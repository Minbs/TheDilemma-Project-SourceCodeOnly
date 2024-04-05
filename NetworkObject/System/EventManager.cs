using System;
using System.Collections.Generic;
using Gameplay;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using UnityEngine;

public class EventManager : NetworkObject
{
	public override NetObjectType Type => NetObjectType.System_EventManager;

	private GameplayManager mGameplayManager;

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	[SyncVar]
	public readonly SyncField<NetFloat> mEndpointPrograss = new();
	private bool mIsEndpointReached = false;
	private float mEndpointPrograssSec = 3.0f;

	/// <summary>유저들이 마지막 장소에 도착한 경우, 맵 전환시까지의 진행도를 표시합니다.</summary>
	public float EndpointPrograss
	{
		get
		{
			float prograss = mEndpointPrograss.Data / mEndpointPrograssSec;
			if (prograss > 1)
			{
				prograss = 1;
			}
			return prograss;
		}
	}

	/// <summary>진행중이던 이벤트를 모두 초기화 합니다.</summary>
	public void ResetEvents()
	{
		mEndpointPrograss.Data = 0;
	}

	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (mIsEndpointReached)
		{
			mEndpointPrograss.Data += deltaTimeInfo.ScaledDeltaTime;
			if (mEndpointPrograss.Data > mEndpointPrograssSec)
			{
				mGameplayManager.Server_ChangeMap(MapType.map_underground_stage_2);
			}
		}
	}

	/// <summary>플레이어들이 마지막 장소에 도착하면 호출됩니다.</summary>
	public void OnPlayerReachedEndpoint()
	{
		mIsEndpointReached = true;
	}

	public void OnNoPlayerDetectedAtEndPoint()
	{
		mIsEndpointReached = false;
	}
}
