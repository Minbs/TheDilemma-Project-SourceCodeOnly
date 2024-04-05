using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using MonoGUI;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Utils.Service;

public class GlobalGuiService : MonoService
{
	[TitleGroup("GUI Services")]
	[SerializeField] private Navigation_EscapeMenu GUI_Navigation_EscapeMenu;
	[SerializeField] private Navigation_SystemDialog GUI_Navigation_SystemDialog;
	[SerializeField] private Navigation_Loading GUI_Navigation_Loading;
	//[SerializeField] private Navigation_Debug GUI_Navigation_Debug;

	public bool CanEscape => !GUI_Navigation_EscapeMenu.HasView && !GUI_Navigation_SystemDialog.HasView;

	// Global GUI Navigation
	private readonly BidirectionalMap<Type, MonoGUI_Navigation> mGlobalGuiNavigation = new();

	public override void OnRegistered()
	{
		base.OnRegistered();

		GUI_Navigation_EscapeMenu.Initialize();
		bindGlobalGuiNavigation(GUI_Navigation_EscapeMenu);

		GUI_Navigation_SystemDialog.Initialize();
		bindGlobalGuiNavigation(GUI_Navigation_SystemDialog);

		//GUI_Navigation_Debug.Initialize();
		//bindGlobalGuiNavigation(GUI_Navigation_Debug);

		void bindGlobalGuiNavigation(MonoGUI_Navigation navigation)
		{
			if (!mGlobalGuiNavigation.TryAdd(navigation.GetType(), navigation))
			{
				Ulog.LogError
				(
					UlogType.GlobalServiceLocator,
					$"BindGlobalGuiNavigation Failed! GlobalGUI type : {navigation.GetType().Name}"
				);
			}
		}

		var escapeAction = GlobalServiceLocator.InputService
			.GetServiceOrNull()
			.GetInputAction(InputType.Key_Escape);

		escapeAction.OnPressed += onEscapePressed;
	}

	public override void OnUnregistered()
	{
		var escapeAction = GlobalServiceLocator.InputService
			.GetServiceOrNull()
			.GetInputAction(InputType.Key_Escape);

		escapeAction.OnPressed -= onEscapePressed;

		base.OnUnregistered();
	}

	private void onEscapePressed()
	{
		// 시스템 메세지가 열린 상태에서는 Escape키로 나갈 수 없다.
		if (GUI_Navigation_SystemDialog.HasView)
		{
			return;
		}

		if (GUI_Navigation_EscapeMenu.HasView)
		{
			GUI_Navigation_EscapeMenu.CloseEscapeMenu();
		}
		else
		{
			GUI_Navigation_EscapeMenu.OpenEscapeMenu();
		}
	}

	public T GetGlobalGuiNavigation<T>() where T : MonoGUI_Navigation
	{
		if (mGlobalGuiNavigation.TryGetValue(typeof(T), out MonoGUI_Navigation navigation))
		{
			return navigation as T;
		}

		return null;
	}

	public View_SystemDialog ShowSystemDialog
	(
		NetOperationType operationType,
		string title,
		string context,
		params (DialogResult Result, bool IsInteractable)[] dialogResults
	)
	{
		var dialogNav = GetGlobalGuiNavigation<Navigation_SystemDialog>();
		return dialogNav.ShowSystemDialog(operationType, title, context, dialogResults);
	}

	public void SetupEscapeButtons(IList<EscapeButtonInfo> buttonInfos)
	{
		GUI_Navigation_EscapeMenu.SetupButtons(buttonInfos);
	}

	public void CloseEscapeMenu()
	{
		GUI_Navigation_EscapeMenu.CloseEscapeMenu();
	}

	public void OpenSetting()
	{
		GUI_Navigation_EscapeMenu.OpenOption();
	}

	/// <summary>게임 플레이 도중에 종료를 시도합니다.</summary>
	public void QuitGameWhilePlaying()
	{
		var view = ShowSystemDialog
		(
			NetOperationType.QuitGame,
			NetOperationType.QuitGame.GetTitle(),
			"게임을 종료하면 현재 진행상황을 잃게 됩니다.\n그래도 종료하시겠습니까?",
			(DialogResult.OK, true),
			(DialogResult.Cancel, true)
		);

		view.OnResultCallback += (result) =>
		{
			if (result == DialogResult.OK)
			{
				ProcessHandler.Instance.StopProcess();
			}
		};
	}

	/// <summary>종료를 시도합니다.</summary>
	public void QuitGame()
	{
		var view = ShowSystemDialog
		(
			NetOperationType.QuitGame,
			NetOperationType.QuitGame.GetTitle(),
			"게임을 종료하시겠습니까?",
			(DialogResult.OK, true),
			(DialogResult.Cancel, true)
		);

		view.OnResultCallback += (result) =>
		{
			if (result == DialogResult.OK)
			{
				ProcessHandler.Instance.StopProcess();
			}
		};
	}

	/// <summary>로딩 GUI가 보여집니다.</summary>
	/// <param name="callback">화면이 완전히 가려지고 호출합니다.</param>
	public void OpenGUILoading(Action callback)
	{
		GUI_Navigation_Loading.OpenGUILoading(callback);
	}

	/// <summary>로딩 GUI가 사라집니다.</summary>
	/// <param name="callback">화면이 완전히 보여지고 호출합니다.</param>
	public void CloseGUILoading(Action callback = null)
	{
		GUI_Navigation_Loading.CloseGUILoading(callback);
	}
}
