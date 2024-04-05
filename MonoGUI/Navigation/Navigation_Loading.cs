using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using System;

public class Navigation_Loading : MonoGUI_Navigation
{
	public void OpenGUILoading(Action action)
	{
		Switch<View_Loading>(action);
	}

	public void CloseGUILoading(Action action = null)
	{
		Pop<View_Loading>(action);
	}
}
