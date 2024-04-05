using System;
using System.Collections;
using Utils;
using Utils.Service;

public class GUiService : IServiceable
{
	#region GUIs

	#endregion

	public void OnRegistered()
	{
		Ulog.Log(this, "OnRegistered");
	}

	public void OnUnregistered()
	{
		Ulog.Log(this, "OnUnregistered");
	}

	public void SwitchGUI(NewGUIView hideView, NewGUIView showView, Action showCallback = null, Action hideCallback = null)
	{
		hideView.Hide(() =>
		{
			hideCallback?.Invoke();
			showView.Show(showCallback);
		});
	}

}
