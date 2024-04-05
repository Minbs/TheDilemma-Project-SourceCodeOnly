using System;
using System.Collections;
using System.Collections.Generic;
public class GUIPopupNavigation
{
	private Dictionary<Type, GUIPopupView> mNavigationTable;

	private GUIPopupView mCurrentView;

	private bool mIsSwitchingAvailable;

	public GUIPopupNavigation()
	{
	}

	public void Initialize()
	{
		mNavigationTable = new();
		mIsSwitchingAvailable = true;

	}

	public void AddPopupView(GUIPopupView view)
	{
		mNavigationTable.Add(view.GetType(), view);
	}

	public void Open<T>() where T : GUIPopupView
	{
		if (mNavigationTable.TryGetValue(typeof(T), out var view))
		{
			mCurrentView?.Close();

			view.Open();
			mCurrentView = view;
		}
	}

	public IEnumerator ViewSwitching<T>(Action<T> onSwitchingCallback) where T : GUIPopupView
	{
		GUIPopupView nextView;

		if (mNavigationTable.TryGetValue(typeof(T), out var view))
		{
			nextView = view as T;

			if (nextView == null)
			{
				yield break;
			}
		}
		else
		{
			yield break;
		}

		if (!mIsSwitchingAvailable)
			yield break;

		mIsSwitchingAvailable = false;

		mCurrentView?.Hide();
		while (mCurrentView?.State == VisableState.Disappearing)
			yield return null;


		nextView?.Show();
		while (nextView?.State == VisableState.Appearing)
			yield return null;

		mCurrentView = nextView;

		// Switching Callback
		var callbackView = mCurrentView as T;
		onSwitchingCallback?.Invoke(callbackView);

		mIsSwitchingAvailable = true;
	}

}
