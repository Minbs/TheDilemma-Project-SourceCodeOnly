using System;
using MonoGUI;
using UnityEngine;

public class View_worldWeaponInfo : View_WorldGUIView
{
	private Vector2 mWorldPosition;
	
	
	public void Initialize(Action onClose, Vector2 worldPosition)
	{
		mWorldPosition = worldPosition;
	}

	public override void OnInitialized()
	{
		
	}

	public void WorldGUIUpdate()
	{
		var screenPos = Camera.main.WorldToScreenPoint(mWorldPosition);
		ViewRectTransfrom.anchoredPosition = screenPos;
	}
}