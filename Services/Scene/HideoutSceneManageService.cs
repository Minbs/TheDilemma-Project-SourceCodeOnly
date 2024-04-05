using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using KaNet.Synchronizers;
using MonoGUI;
using UnityEngine;
using Utils;
using Utils.Service;

public class HideoutSceneManageService : SceneManageService
{
	[field : SerializeField] public SceneType CurrentScene { get; private set; } = SceneType.None;
	[field : SerializeField] public Transform NetworkObjectPoolTransform { get; private set;}

	public readonly ServiceLocator<MonoObjectPoolService> MonoObjectPoolService = new();
	[field : SerializeField] public Navigation_Hideout HideoutNavigation { get; private set; }

	// DI Field
	private NetworkManageService mNetworkManageService;
	private InputService mInputService;

	protected override void bindAllService()
	{
		this.bindServiceLocator(MonoObjectPoolService);
	}

	protected override void onInitialize()
	{
		// Get Services
		if (!GlobalServiceLocator.NetworkManageService.TryGetService(out mNetworkManageService))
		{
			Ulog.LogError(this, $"There is no {mNetworkManageService.GetType().Name}!");
			return;
		}

		if (!GlobalServiceLocator.InputService.TryGetService(out mInputService))
		{
			Ulog.LogError(this, $"There is no {mInputService.GetType().Name}!");
			return;
		}

		// Setup Escape GUI
		var sceneLoader = GlobalServiceLocator.AsyncSceneLoadService.GetServiceOrNull();
		var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();

		globalGui.SetupEscapeButtons(new List<EscapeButtonInfo>
		{
			new EscapeButtonInfo
			(
				"타이틀로 돌아가기",
				() =>{
					sceneLoader.TryLoadSceneAsync(SceneType.scn_game_title);
					globalGui.CloseEscapeMenu();
				},
				false),
			new EscapeButtonInfo("게임 설정", globalGui.OpenSetting, false),
			new EscapeButtonInfo("종료", globalGui.QuitGame, false),
			new EscapeButtonInfo("은신처 터미널", OpenHideoutTerminal, true),
		});

		// Setup Object Pool Service
		MonoObjectPoolService.RegisterService(new MonoObjectPoolService(NetworkObjectPoolTransform));

		// Get instances
		var netObjs = GetComponentsInChildren<NetworkObject>();
		foreach (var no in netObjs)
		{
			mNetworkManageService.ObjectManager.AddInitialNetworkObject(no);
		}

		// Startup network synchronize
		mNetworkManageService.OnSceneLoaded
		(
			CurrentScene,
			MonoObjectPoolService.GetServiceOrNull()
		);
	}

	public void OpenHideoutTerminal()
	{
		var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();
		globalGui.CloseEscapeMenu();
		HideoutNavigation.OpenHideoutTerminal();
	}
}
