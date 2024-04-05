using FMOD.Studio;
using UnityEngine;
using Utils;
using Utils.Service;

public abstract class BaseFmodSoundService : IServiceable
{
	public abstract void Initialize();
	public abstract void OnRegistered();
	public abstract void OnUnregistered();
	public abstract void Play(SoundType type, params SoundParameter[] parmeters);
	public abstract void Play(SoundType type, Vector2 position, params SoundParameter[] parmeters);
	public abstract FmodEvent InstanceSound(SoundType type);
	public abstract void Stop(SoundType type, FMOD.Studio.STOP_MODE mode);
	public abstract void Pause(SoundType type);
	public abstract void Resume(SoundType type);
	public abstract bool IsCurrentlyPlay(SoundType type);
	public abstract void SetSlowPitch(float intensity);
	public abstract void StopSlowPitch();

	/// <summary> 사운드 Master 볼륨입니다. 0.0f ~ 1.0f 값만 들어갑니다.</summary>
	public abstract float MasterVolume { set; get; }

	/// <summary> 사운드 BGM 볼륨입니다. 0.0f ~ 1.0f 값만 들어갑니다.</summary>
	public abstract float BgmVolume { set; get; }

	/// <summary> 사운드 Sfx 볼륨입니다. 0.0f ~ 1.0f 값만 들어갑니다.</summary>
	public abstract float SfxVolume { set; get; }

	/// <summary> 사운드 Ambient 볼륨입니다. 0.0f ~ 1.0f 값만 들어갑니다.</summary>
	public abstract float AmbientVolume {set; get; }

}

public class FmodSoundService : BaseFmodSoundService
{
	private FMOD.Studio.VCA mMaster;
	private FMOD.Studio.VCA mBgm;
	private FMOD.Studio.VCA mSFX;
	private FMOD.Studio.VCA mAmbient;

	private const string mMasterPath = "vca:/Master";
	private const string mBgmPath = "vca:/Bgm";
	private const string mAmbiencePath = "vca:/Ambience";
	private const string mSFXPath = "vca:/SFX";

	private const string mSlowPath = "snapshot:/SlowMotion";

	private FmodEvent mSlowEvent;

	public override float MasterVolume
	{
		get
		{
			mMaster.getVolume(out var volume);
			return volume;
		}
		 set
		{
			value = Mathf.Clamp(value, 0.0f, 1.0f);
			mMaster.setVolume(value);
		}
	}

	public override float BgmVolume
	{
		get
		{
			mBgm.getVolume(out var volume);
			return volume;
		}
		 set
		{
			value = Mathf.Clamp(value, 0.0f, 1.0f);
			mBgm.setVolume(value);
		}
	}

	public override float SfxVolume
	{
		get
		{
			mSFX.getVolume(out var volume);
			return volume;
		}
		 set
		{
			value = Mathf.Clamp(value, 0.0f, 1.0f);
			mSFX.setVolume(value);
		}
	}

	public override float AmbientVolume
	{
		get
		{
			mAmbient.getVolume(out var volume);
			return volume;
		}
		 set
		{
			value = Mathf.Clamp(value, 0.0f, 1.0f);
			mAmbient.setVolume(value);
		}
	}

	public override void Initialize()
	{
		mSlowEvent = new(mSlowPath);

		mMaster = FMODUnity.RuntimeManager.GetVCA(mMasterPath);
		mBgm = FMODUnity.RuntimeManager.GetVCA(mBgmPath);
		mAmbient = FMODUnity.RuntimeManager.GetVCA(mAmbiencePath);
		mSFX = FMODUnity.RuntimeManager.GetVCA(mSFXPath);
	}

	public override void OnRegistered()
	{
		Ulog.Log(this, "OnRegistered");
	}

	public override void OnUnregistered()
	{
		// FMOD 언로드
		Ulog.Log(this, "OnUnregistered");
	}

	public override void Play(SoundType type, params SoundParameter[] parmeters)
	{
		FmodSoundTable.GetFmodEvent(type).Play(parmeters);
	}

	public override void Play(SoundType type, Vector2 position, params SoundParameter[] parmeters)
	{
		FmodSoundTable.GetFmodEvent(type).PlayToPosition(position, parmeters);
	}

	/// <summary> 사운드 테이블에 있는 사운드를 별도로 관리할 때 사용합니다. </summary>
	public override FmodEvent InstanceSound(SoundType type)
	{
		var fmodEvent = FmodSoundTable.GetFmodEvent(type);
		return new FmodEvent(fmodEvent.Path);
	}

	public override void Stop(SoundType type, FMOD.Studio.STOP_MODE mode)
	{
		FmodSoundTable.GetFmodEvent(type).Stop(mode);
	}

	public override void Pause(SoundType type)
	{
		FmodSoundTable.GetFmodEvent(type).Pause();
	}

	public override void Resume(SoundType type)
	{
		FmodSoundTable.GetFmodEvent(type).Resume();
	}

	public override bool IsCurrentlyPlay(SoundType type)
	{
		return FmodSoundTable.GetFmodEvent(type).IsPlaying();
	}

	/// <summary> 사운드 Pitch를 느리게 해줍니다.</summary>
	/// <param name="intensity">intensity 커질수록 Pitch가 점점 느려집니다. 0.0f ~ 100.0f 범위로 값을 넣어줘야됩니다.</param>
	public override void SetSlowPitch(float intensity)
	{
		intensity = Mathf.Clamp(intensity, 0.0f, 100.0f);

		if (!mSlowEvent.IsPlaying())
		{
			mSlowEvent.Play();
		}

		mSlowEvent.SetParmater("Intensity", intensity);
	}

	/// <summary> 사운드 Ptich 느리게하는 효과를 멈춤니다.</summary>
	public override void StopSlowPitch()
	{
		mSlowEvent.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}

public class NullFmodSoundService : BaseFmodSoundService
{
	public override float MasterVolume { 
		get => 0.0f;
		 set
		{
		}
	}
	public override float BgmVolume {
		get => 0.0f;
		 set
		{
		}
	}
	public override float SfxVolume { 
		get => 0.0f;
		 set
		{
		}
	}
	public override float AmbientVolume { 
		get => 0.0f;
		 set
		{
		} 
	}

	public override void Initialize() { }
	public override FmodEvent InstanceSound(SoundType type) { return null; }
	public override bool IsCurrentlyPlay(SoundType type) { return false; }
	public override void OnRegistered() { }
	public override void OnUnregistered() { }
	public override void Pause(SoundType type) { }
	public override void Play(SoundType type, params SoundParameter[] parmeters) { }
	public override void Play(SoundType type, Vector2 position, params SoundParameter[] parmeters) { }
	public override void Resume(SoundType type) { }
	public override void SetSlowPitch(float intensity) { }
	public override void Stop(SoundType type, STOP_MODE mode) { }
	public override void StopSlowPitch() { }
}