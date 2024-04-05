using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public abstract class GUINavigationView : GUIView
{
	protected GUINavigation mNavigation { get; private set; } = null;
	protected Dictionary<Type, GUIPopupView> mPopupViewTable = new Dictionary<Type, GUIPopupView>();

	/// <summary>오버라이딩시에 내부에 ViewSetUp을 호출해주세요.</summary>
	public virtual void NaigationViewSetup(GUINavigation navigation)
	{
		mNavigation = navigation;
		var findedPopupViews = GetComponentsInChildren<GUIPopupView>();

		foreach (var popupView in findedPopupViews)
		{
			popupView.PopupViewSetup();
			mPopupViewTable.Add(popupView.GetType(), popupView);
		}
		viewSetUp();
	}

	public void PopupShow<T>() where T : GUIPopupView
	{
		Type guiType = typeof(T);

		if (!mPopupViewTable.TryGetValue(guiType, out var popupView))
		{
			Ulog.LogError(UlogType.UI, $"Popup open failed! A {guiType.Name} does not exist!");
		}

		popupView.Show();
	}

	public void PopupHide<T>() where T : GUIPopupView
	{
		Type guiType = typeof(T);

		if (!mPopupViewTable.TryGetValue(guiType, out var popupView))
		{
			Ulog.LogError(UlogType.UI, $"Popup open failed! A {guiType.Name} does not exist!");
		}

		popupView.Hide();
	}

	public void PopupAllClose()
	{
		foreach (var popupView in mPopupViewTable.Values)
		{
			popupView.Close();
		}
	}

	public bool TryGetPopupView<T>(out T view) where T : GUIPopupView
	{
		Type guiType = typeof(T);

		if (!mPopupViewTable.TryGetValue(guiType, out var popupView))
		{
			Ulog.LogError(UlogType.UI, $"Popup open failed! A {guiType.Name} does not exist!");
			view = null;
			return false;
		}

		view = popupView as T;
		return view != null;
	}
}
