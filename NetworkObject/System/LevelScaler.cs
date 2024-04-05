using System;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using UnityEngine;

public class LevelScaler : NetworkObject
{
	public override NetObjectType Type => NetObjectType.System_LevelScaler;

	private GameplayManager mGameplayManager;

	/// <summary>다음 레벨로 가는 장소로 가는 Prograss입니다.</summary>
	[SyncVar] public readonly SyncField<NetFloat> PrograssGoToNextLevel = new();

	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
	}

	/// <summary>진행중이던 이벤트를 모두 초기화 합니다.</summary>
	public void ResetEvents()
	{
		PrograssGoToNextLevel.Data = 0;
	}

}
