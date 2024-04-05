using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils.ViewModel;
using Utils;
using Gameplay;
using System;
using KaNet.Synchronizers;
using KaNet.Session;

[Obsolete]
public class PlayerInfo
{
	public CharacterType Character;
	public string Name;
	public int HpMax;
	public int HpCurrent;
	public int StMax;
	public int StCurrent;
	public AmmoInfoType AmmoInfoType;
	public int MagaineBullet;
	public int TotalBullet;
	public bool IsConncetionGood;

	public PlayerInfo(
		CharacterType character, 
		string name, 
		int hpMax, 
		int hpCurrent, 
		int stMax, 
		int stCurrent, 
		AmmoInfoType ammoInfoType, 
		int magaineBullet, 
		int totalBullet, 
		bool isConncetionGood)
	{
		Character = character;
		Name = name;
		HpMax = hpMax;
		HpCurrent = hpCurrent;
		StMax = stMax;
		StCurrent = stCurrent;
		AmmoInfoType = ammoInfoType;
		MagaineBullet = magaineBullet;
		TotalBullet = totalBullet;
		IsConncetionGood = isConncetionGood;
	}
}

public class View_TeamInfo : MonoGUI_View
{
	[SerializeField] private ImageViewModel Img_Portrait = new(nameof(Img_Portrait));
	[SerializeField] private TextMeshProTextViewModel Text_Username = new(nameof(Text_Username));
	[SerializeField] private ImageViewModel Img_ConnectionState = new(nameof(Img_ConnectionState));

	//GagueViewModel
	[SerializeField] private ImageViewModel Img_Main = new(nameof(Img_Main));
	[SerializeField] private ImageViewModel Img_Change = new(nameof(Img_Change));

	private Gague mGague;
	[SerializeField] private float GagueDecreaseSpeed;

	[SerializeField] private Sprite ConnectionGood;
	[SerializeField] private Sprite ConnectionBad;

	[SerializeField] private SerializableDictionary<CharacterType, Sprite> PortraitTable = new();

	public IngameSessionInfo SessionInfo { get; private set; }

	private GameplayManager mGameplayManager;
	private PlayerEntityService mPlayerEntityService;

	public override void OnInitialized()
	{
		Ulog.Log($"[HUD] Draw TeamInfo");
		Img_Portrait.Initialize(this);
		Text_Username.Initialize(this);
		Img_ConnectionState.Initialize(this);
		Img_Main.Initialize(this);
		Img_Change.Initialize(this);
	}

	public void Initialize(GameplayManager gameplayManager, IngameSessionInfo ingameSessionInfo)
	{
		if (PortraitTable.TryGetValue(ingameSessionInfo.Character, out var portrait))
		{
			Img_Portrait.Sprite = portrait;
		}

		Text_Username.Text = ingameSessionInfo.Name;
		mGague = new(this, Img_Main, Img_Change, GagueDecreaseSpeed);

		mGameplayManager = gameplayManager;
		mPlayerEntityService = mGameplayManager.EntityService.PlayerEntityService;
		SessionInfo = ingameSessionInfo;
	}

	public void Update()
	{
		if (mPlayerEntityService.TryGetPlayer(SessionInfo.ID, out var player))
		{
			Img_Portrait.Color = new Color(1, 1, 1);
			mGague.UpdateValue(player.HP, player.MaxHP);
		}
		else
		{
			Img_Portrait.Color = new Color(0.2f, 0.2f, 0.2f);
			mGague.UpdateValue(0, 100);
		}
	}
}
