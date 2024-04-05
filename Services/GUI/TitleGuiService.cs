using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Service;
using UnityEngine;

using MonoGUI;

public class TitleGuiService : MonoService
{
	[field : SerializeField]
	public MonoGUI_Navigation GUI_TitleNavigation { get; private set; }

	private InputService mInputService;

	public override void OnRegistered()
	{
		base.OnRegistered();

		Ulog.Log(this, $"On Registered");

		if (!GlobalServiceLocator.InputService.TryGetService(out mInputService))
		{
			Ulog.LogError(this, $"There is no {typeof(InputService).Name} in the current scene.");
		}

		mInputService.GetInputAction(InputType.Key_Escape).OnReleased += OnEscape;

		var globalGui = GlobalServiceLocator
			.GlobalGuiService
			.GetServiceOrNull();
		
		globalGui.SetupEscapeButtons(
			new List<EscapeButtonInfo>
		{ 
			new EscapeButtonInfo("게임 설정", globalGui.OpenSetting, false),
			new EscapeButtonInfo("종료", globalGui.QuitGame, false),
		});

		OnGUI_GotoTitleMain();
	}

	public override void OnUnregistered()
	{
		mInputService.GetInputAction(InputType.Key_Escape).OnReleased -= OnEscape;

		base.OnUnregistered();
	}

	public void OnEscape()
	{

	}

	public void OnGUI_GotoTitleMain()
	{
		GUI_TitleNavigation.Push<View_TitleMenu>();
	}

	public void OnGUI_GotoOption()
	{
	}
}
