using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;
using TMPro;
using Utils;

public class View_Preferences : GUINavigationView
{
	[SerializeField]
	private TMP_DropdownViewModel Dropdown_Resoulution = new(nameof(Dropdown_Resoulution));

	private Resolution [] mResoulutions;

	[SerializeField]
	private ToggleViewModel Toggle_FullScreen = new(nameof(Toggle_FullScreen));

	[SerializeField]
	private TMP_DropdownViewModel Dropdown_Language = new(nameof(Dropdown_Language));

	[SerializeField]
	private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));

	private int mCurrentResolutionIndex;

	private readonly int mRefreshRate_144 = 144;

	public override void NaigationViewSetup(GUINavigation navigation)
	{
		base.NaigationViewSetup(navigation);

		#region Resoulution

		Toggle_FullScreen.Initialize(this);

		Toggle_FullScreen.IsOn = Screen.fullScreen;

		Toggle_FullScreen.BindAction((isFullScreen) =>
			{
				setResoulution(mResoulutions[mCurrentResolutionIndex]);
			});

		Dropdown_Resoulution.Initialize(this);

		mResoulutions = Screen.resolutions;

		for (int i = 0; i < mResoulutions.Length; i++)
		{
			TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData();
			item.text = $"{mResoulutions[i].width} X {mResoulutions[i].height} : {mResoulutions[i].refreshRate} Hz";
			Dropdown_Resoulution.AddOption(item);
			if(Screen.width == mResoulutions[i].width &&
				Screen.height == mResoulutions[i].height &&
				Screen.currentResolution.refreshRate == mRefreshRate_144)
			{
				mCurrentResolutionIndex = i;
			}
		}

		Dropdown_Resoulution.Value = mCurrentResolutionIndex;

		Dropdown_Resoulution.BindAction((index) =>
		{
			setResoulution(mResoulutions[index]);
			mCurrentResolutionIndex = index;
		});

		#endregion

		#region 언어 설정부분 제작 필요
		#endregion

		Btn_Exit.Initialize(this);
		Btn_Exit.BindAction(Exit);
	}

	private void setResoulution(Resolution resolution)
	{
		Screen.SetResolution(resolution.width, resolution.height, Toggle_FullScreen.IsOn);
		Ulog.Log(UlogType.UI, $"{Screen.currentResolution.width} X {Screen.currentResolution.height} : {Screen.currentResolution.refreshRate} Hz, FullScreen: {Screen.fullScreen}");
	}

	private void Exit()
	{
		mNavigation.TryPop(out var topView);
	}
}
