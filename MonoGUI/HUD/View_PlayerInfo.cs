using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils;
using Utils.ViewModel;
using System;
using Gameplay;
using KaNet.Synchronizers;

//[Obsolete]
public enum AmmoInfoType
{
	Pistol,
	Rifle,
	Heavy,
	Shotgun,
	Energy,
}

public class View_PlayerInfo : MonoGUI_View
{
	[SerializeField] private ImageViewModel Img_Profile = new(nameof(Img_Profile));
	//HP
	[SerializeField] private ImageViewModel Img_HpMain = new(nameof(Img_HpMain));
	[SerializeField] private ImageViewModel Img_HpChange = new(nameof(Img_HpChange));
	private Gague mHpGague;
	//ST
	[SerializeField] private ImageViewModel Img_StMain = new(nameof(Img_StMain));
	[SerializeField] private ImageViewModel Img_StChange = new(nameof(Img_StChange));
	private Gague mStGague;
	//Bullet
	[SerializeField] private TextMeshProTextViewModel Text_MagazineBullet = new(nameof(Text_MagazineBullet));
	[SerializeField] private TextMeshProTextViewModel Text_TotalBullet = new(nameof(Text_TotalBullet));
	[SerializeField] private ImageViewModel Img_BulletType = new(nameof(Img_BulletType));

	private ResourcesService mResourceService
		=> GlobalServiceLocator.ResourcesService.GetServiceOrNull();

	private GameplayManager mGameplayManager;
	private PlayerEntityService mPlayerEntityService;

	public override void OnInitialized()
	{
		Img_Profile.Initialize(this);
		Img_HpMain.Initialize(this);
		Img_HpChange.Initialize(this);
		Img_StMain.Initialize(this);
		Img_StChange.Initialize(this);
		Text_MagazineBullet.Initialize(this);
		Text_TotalBullet.Initialize(this);
		Img_BulletType.Initialize(this);
	}

	public void Initialize(GameplayManager gameplayManager, IngameSessionInfo ingameSessionInfo)
	{
		mGameplayManager = gameplayManager;

		mPlayerEntityService = mGameplayManager.EntityService.PlayerEntityService;
		mHpGague = new(this, Img_HpMain, Img_HpChange, 0.7f);

		Img_Profile.Sprite = mResourceService.ProfileTable[ingameSessionInfo.Character];
		//Img_BulletType.Sprite = mResourceService.AmmoInfoTable[AmmoInfoType.Pistol];
	}

	public void Update()
	{
		if (mPlayerEntityService.TryGetClientPlayer(out var player))
		{
			mHpGague.UpdateValue(player.HP, player.MaxHP);

			SetMagazineBullet(player.CurrentEquipmentState.Magazine);
			SetTotalBullet(player.CurrentEquipmentState.AmmoOwned);
		}
		else
		{
			mHpGague.UpdateValue(0, 100);
		}
	}

	public void SetMagazineBullet(int amount)
	{
		Text_MagazineBullet.Text = amount.ToString();
	}

	public void SetTotalBullet(int amount)
	{
		Text_TotalBullet.Text = amount > 10000 ? "¡Ä" : amount.ToString();
	}
}
