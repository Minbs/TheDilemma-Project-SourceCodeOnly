using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using Sirenix.OdinInspector;
using UnityEngine;
using MonoGUI;
using System;
using DG.Tweening;

public class GlobalMonoServiceInitializer : MonoBehaviour, IProcessInitializable
{
	[TitleGroup("System Services")]
	[SerializeField] private SystemInformationService mSystemInformationService;
	[SerializeField] private OptionService mOptionService;
	[SerializeField] private InputService mInputService;
	[SerializeField] private ResourcesService mResourcesService;
	[SerializeField] private NetworkManageService mNetworkManageService;
	[SerializeField] private GlobalGuiService mGlobalGuiService;
	[SerializeField] private VirtualMouseService mVirtualMouseService;

	public void InitializeByProcessHandler(ProcessHandler gameProcessHandler)
	{
		//Tween
		DOTween.Init();
		DOTween.defaultAutoPlay = AutoPlay.None;

		// System Information Service
		try
		{
			GlobalServiceLocator.SystemInformationService.RegisterService(mSystemInformationService);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{mSystemInformationService.GetType().Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Option Service
		try
		{
			GlobalServiceLocator.OptionService.RegisterService(mOptionService);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{mOptionService.GetType().Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Register Input
		try
		{
			GlobalServiceLocator.InputService.RegisterService(mInputService);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{mInputService.GetType().Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Resource Service
		try
		{
			GlobalServiceLocator.ResourcesService.RegisterService(mResourcesService);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{mResourcesService.GetType().Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Network Manage Service
		try
		{
			GlobalServiceLocator.NetworkManageService.RegisterService(mNetworkManageService);
			var sceneService = GlobalServiceLocator.AsyncSceneLoadService.GetServiceOrNull();
			mNetworkManageService.Initialized(gameProcessHandler.ID, sceneService, mResourcesService.NetworkObjectTable);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{mNetworkManageService.GetType().Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Global GUI Service
		try
		{
			GlobalServiceLocator.GlobalGuiService.RegisterService(mGlobalGuiService);
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{mGlobalGuiService.GetType().Name} initialize error!");
			gameProcessHandler.AddProcessInitialMessage(e.Message);
		}

		// Virtual Mouse Service
		GlobalServiceLocator.VirtualMouse.RegisterService(mVirtualMouseService);
	}
}
