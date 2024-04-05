using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using MonoGUI;
using UnityEngine;
using Utils;
using Utils.Service;

public enum GlobalGUI
{
	None = 0,
	SystemDialog,
	SceneChanger,
}

/// <summary>전역 서비스 중개자입니다.</summary>
public static class GlobalServiceLocator
{
	/// <summary>전역 서비스 중개자를 초기화합니다.</summary>
	/// <param name="gameProcessHandler">함수를 호출한 Mono 게임 프로세스 컨트롤러 클래스입니다.</param>
	public static void InitializeByProcessHandler(ProcessHandler gameProcessHandler)
	{
		// Unregister services when process stopped.
		gameProcessHandler.OnStopProcess += () =>
		{
			for (int i = mGlobalServices.Count - 1; i >= 0; i--)
			{
				mGlobalServices[i]?.UnregisterService();
			}
		};

		// Cache program ID
		var programeID = gameProcessHandler.ID;

		// Register async scene loading service
		try
		{
			AsyncSceneLoadService sceneService = new AsyncSceneLoadService();
			sceneService.BindResetAction(() =>
			{
				MonoObjectPoolService.UnregisterService();
				SceneManageService.UnregisterService();
			});
			AsyncSceneLoadService.RegisterService(sceneService);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{typeof(AsyncSceneLoadService).Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Register sound service
		try
		{
			var soundServiceInstance = new FmodSoundService();
			soundServiceInstance.Initialize();
			SoundService.RegisterService(soundServiceInstance);
			Ulog.Log(UlogType.GlobalServiceLocator, $"Fmod Service Registered!");
		}
		catch (Exception e)
		{
			Ulog.LogError(UlogType.GlobalServiceLocator, $"Null Fmod Service Registered!");
			Ulog.LogError(UlogType.GlobalServiceLocator, e);
			var nullSoundService = new NullFmodSoundService();
			SoundService.RegisterService(nullSoundService);
		}
	}

	// Service Locators
	public static readonly SceneManageServiceLocator SceneManageService = new();
	public static readonly ServiceLocator<SystemInformationService> SystemInformationService = new();
	public static readonly ServiceLocator<OptionService> OptionService= new();
	public static readonly ServiceLocator<InputService> InputService = new();
	public static readonly ServiceLocator<NetworkManageService> NetworkManageService = new();
	public static readonly ServiceLocator<GlobalGuiService> GlobalGuiService = new();
	public static readonly ServiceLocator<VirtualMouseService> VirtualMouse = new();

	public static readonly ServiceLocator<AsyncSceneLoadService> AsyncSceneLoadService = new();
	public static readonly ServiceLocator<MonoObjectPoolService> MonoObjectPoolService = new();
	public static readonly ServiceLocator<ResourcesService> ResourcesService = new();

	public static readonly ServiceLocator<BaseFmodSoundService> SoundService = new();

	public static readonly ServiceLocator<LocalizationService> LocalizationService = new();

	private static readonly List<IUnregistrable> mGlobalServices = new()
	{
		SceneManageService,
		SystemInformationService,
		OptionService,
		InputService,
		NetworkManageService,
		GlobalGuiService,
		VirtualMouse,

		AsyncSceneLoadService,
		MonoObjectPoolService,
		ResourcesService,

		SoundService,
		LocalizationService,
	};
}
