using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using System;
using Sirenix.OdinInspector;
using Utils.ViewModel;
public class View_Option : MonoGUI_View
{
	private Action mOnClose;

	[SerializeField] private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));

	//Resoulution
	[SerializeField] private TMP_DropdownViewModel Dropdown_Resoulution = new(nameof(Dropdown_Resoulution));
	[SerializeField] private ToggleViewModel Toggle_FullScreen = new(nameof(Toggle_FullScreen));
	//Language
	[SerializeField] private TMP_DropdownViewModel Dropdown_Language = new(nameof(Dropdown_Language));
	//Sound
	[SerializeField] private SliderViewModel Slider_Master = new(nameof(Slider_Master));
	[SerializeField] private SliderViewModel Slider_Bgm = new(nameof(Slider_Bgm));
	[SerializeField] private SliderViewModel Slider_Sfx = new(nameof(Slider_Sfx));
	[SerializeField] private SliderViewModel Slider_Ambient = new(nameof(Slider_Ambient));

	private BaseFmodSoundService mSoundService;

	public void Initialize(Action onClose)
	{
		Btn_Exit.BindAction(() => onClose?.Invoke());

		//SetUp Sound
		Slider_Master.Value = mSoundService.MasterVolume;
		Slider_Bgm.Value = mSoundService.BgmVolume;
		Slider_Sfx.Value = mSoundService.SfxVolume;
		Slider_Ambient.Value = mSoundService.AmbientVolume;

		Slider_Master.BindAction((volume) => mSoundService.MasterVolume = volume);
		Slider_Bgm.BindAction((volume) => mSoundService.BgmVolume = volume);
		Slider_Sfx.BindAction((volume) => mSoundService.SfxVolume = volume);
		Slider_Ambient.BindAction((volume) => mSoundService.AmbientVolume = volume);
	}

	public override void OnInitialized()
	{
		Btn_Exit.Initialize(this);
		Dropdown_Resoulution.Initialize(this);
		Toggle_FullScreen.Initialize(this);
		Dropdown_Language.Initialize(this);
		Slider_Master.Initialize(this);
		Slider_Bgm.Initialize(this);
		Slider_Sfx.Initialize(this);
		Slider_Ambient.Initialize(this);

		mSoundService = GlobalServiceLocator.SoundService.GetServiceOrNull();
	}
}
