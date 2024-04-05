using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
public class Navigation_TitleMenu_OptionWindow : MonoGUI_Navigation
{
	public void OnOpenOption()
	{
		var view = Push<View_Option>();
		view.Initialize(OnCloseOption);
	}

	public void OnCloseOption()
	{
		Pop<View_Option>();
	}
}
