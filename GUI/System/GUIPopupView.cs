using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GUIPopupView : GUIView
{
	public virtual void PopupViewSetup()
	{
		viewSetUp();
	}

	public void PopupClose(Action closeCallback)
	{
		Close();

		closeCallback?.Invoke();
	}

	public IEnumerator PopupHide(Action hideCallback)
	{
		Hide();
		while (State == VisableState.Disappearing)
			yield return null;

		hideCallback?.Invoke();
	}
}
