using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
public class Navigation_WorldGUI : MonoGUI_Navigation
{
	private RectTransform mDrawRectTransform;

	private void Awake()
	{
		mDrawRectTransform = GetComponent<RectTransform>();
	}

	public void OpenWorldGUI<T>(Transform target,bool isOverDraw, out T view) where T : ToolTip_WorldView
	{
		var worldView = Push<T>();

		worldView.InitializeWorldToolTip(
			mDrawRectTransform, 
			target, 
			isOverDraw,
			()=>PopByObject(worldView.gameObject));

		view = worldView;
	}


}
