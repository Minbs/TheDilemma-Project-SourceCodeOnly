using MonoGUI;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class View_IngameAlive : MonoGUI_View
{
	[field: SerializeField] public Navigation_HUD HUDNavigation { private set; get; }

	public View_Chat Chat { private set; get; }
	public Dictionary<string, View_TeamInfo> TeamInfos { private set; get; }

	private GameplayManager mGameplayManager;

	[Title("ItemSlot"), SerializeField] private List<Sprite> mItemSlot;

	public override void OnInitialized() { }

	public override void OnShow()
	{
		base.OnShow();
	}

	#region Creation and Deletion

	/// <summary>View를 초기화합니다.</summary>
	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;
		HUDNavigation.OpenHUD(mGameplayManager);
	}

	/// <summary>View가 해제될 때 호출됩니다.</summary>
	public override void OnHide()
	{
		HUDNavigation.Clear();
	}

	#endregion

	public void DrawSystemMessage(MessageType type, string msg, float durlation)
	{
		HUDNavigation.DrawSystemMessage(type, msg, durlation);
	}
}
