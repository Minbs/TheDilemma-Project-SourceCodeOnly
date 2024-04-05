using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
public class Navigation_ScreenGUI : MonoGUI_Navigation
{
	private RectTransform mDrawRectTransform;

	private void Awake()
	{
		mDrawRectTransform = GetComponent<RectTransform>();
	}

	public void OpenScreenGUI<T>(Vector2 screenPosition, bool isOverDraw, out T view) where T : ToolTip_ScreenView
	{
		var screenView = Push<T>();

		screenView.InitializeWorldToolTip(
			mDrawRectTransform,
			screenPosition, 
			isOverDraw,
			()=>PopByObject(screenView.gameObject));

		view = screenView;
	}


}
