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

public class IngameSceneManageService : SceneManageService
{
	[field : SerializeField] public SceneType CurrentScene { get; private set; } = SceneType.None;
	[field : SerializeField] public Transform NetworkObjectPoolTransform { get; private set;}

	//public readonly ServiceLocator<MonoObjectPoolService> MonoObjectPoolService = new();
	[field : SerializeField] public Navigation_Ingame IngameNavigation { get; private set; }

	// DI Field
	private NetworkManageService mNetworkManageService;
	private InputService mInputService;

	protected override void bindAllService()
	{
		//this.bindServiceLocator(MonoObjectPoolService);
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
			new EscapeButtonInfo("게임 설정", globalGui.OpenSetting, false),
			new EscapeButtonInfo("메인메뉴로 돌아가기", ()=>
			{
				var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();
				var dialog = globalGui.ShowSystemDialog
				(
					NetOperationType.ReturnToTitle,
					NetOperationType.ReturnToTitle.GetTitle(),
					"메인메뉴로 돌아가면 현재 진행상황을 잃게 됩니다.\n그래도 돌아가시겠습니까?",
					(DialogResult.OK, true),
					(DialogResult.Cancel, true)
				);

				dialog.OnResultCallback += (result) =>
				{
					if (result == DialogResult.OK)
					{
						globalGui.CloseEscapeMenu();
						mNetworkManageService.Disconnect();
					}
				};

			}, false),
			new EscapeButtonInfo("터미널", OpenIngameTerminal, true),
		});

		//// Setup Object Pool Service
		//MonoObjectPoolService.RegisterService(new MonoObjectPoolService(NetworkObjectPoolTransform));

		GlobalServiceLocator.MonoObjectPoolService.RegisterService(new MonoObjectPoolService(transform));

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
			GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()
		);

		GlobalServiceLocator.MonoObjectPoolService.RegisterService(new MonoObjectPoolService(transform));
	}

	public void OpenIngameTerminal()
	{
		var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();
		globalGui.CloseEscapeMenu();
	}
}
