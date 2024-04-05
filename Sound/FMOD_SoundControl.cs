using UnityEngine;
using Utils.ViewModel;
public class FMOD_SoundControl : MonoBehaviour
{
	private FMOD.Studio.VCA mMaster;
	private FMOD.Studio.VCA mBgm;
	private FMOD.Studio.VCA mSFX;
	private FMOD.Studio.VCA mAmbient;

	private string mMasterPath = "vca:/Master";
	private string mBgmPath = "vca:/Bgm";
	private string mAmbiencePath = "vca:/Ambience";
	private string mSFXPath = "vca:/SFX";

	[SerializeField]
	private SliderViewModel Slider_Master = new(nameof(Slider_Master));
	[SerializeField]
	private SliderViewModel Slider_BGM = new(nameof(Slider_BGM));
	[SerializeField]
	private SliderViewModel Slider_SFX = new(nameof(Slider_SFX));
	[SerializeField]
	private SliderViewModel Slider_Ambience = new(nameof(Slider_Ambience));
	[SerializeField]
	private SliderViewModel Slider_Pitch = new(nameof(Slider_Pitch));

	private BaseFmodSoundService mSoundService;

	public void Awake()
	{
		mSoundService = GlobalServiceLocator.SoundService.GetServiceOrNull();

		mMaster = FMODUnity.RuntimeManager.GetVCA(mMasterPath);
		mBgm = FMODUnity.RuntimeManager.GetVCA(mBgmPath);
		mAmbient = FMODUnity.RuntimeManager.GetVCA(mAmbiencePath);
		mSFX = FMODUnity.RuntimeManager.GetVCA(mSFXPath);

		Slider_Master.Initialize(this);
		mMaster.getVolume(out var masterVolume);
		Slider_Master.Value = masterVolume;
		Slider_Master.BindAction(
			value =>
			{
				mMaster.setVolume(value);
			});

		Slider_BGM.Initialize(this);
		mBgm.getVolume(out var bgmVolume);
		Slider_BGM.Value = bgmVolume;
		Slider_BGM.BindAction(
			value =>
			{
				mBgm.setVolume(value);
			});

		Slider_SFX.Initialize(this);
		mSFX.getVolume(out var sfxVolume);
		Slider_SFX.Value = sfxVolume;
		Slider_SFX.BindAction(
			value =>
			{
				mSFX.setVolume(value);
			});

		Slider_Ambience.Initialize(this);
		mAmbient.getVolume(out var ambientVolume);
		Slider_Ambience.Value = ambientVolume;
		Slider_Ambience.BindAction(
			value =>
			{
				mAmbient.setVolume(value);
			});

		Slider_Pitch.Initialize(this);
		Slider_Pitch.Value = 1.0f;//mSoundService.GetPitch();
		Slider_Pitch.BindAction(
			value =>
			{
				mSoundService.SetSlowPitch(value * 100.0f);
			});

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
			mSoundService.StopSlowPitch();
	}

}
