using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class FmodEvent
{
	public string Path { private set; get; }

	//private EventReference mReference;
	private EventDescription mDescription;
	private EventInstance mInstance;
	
	private bool mIsOneShot;

	public FmodEvent(string path)
	{
		Path = path;
		//mReference = EventReference.Find(Path);
		//mDescription = RuntimeManager.GetEventDescription(mReference);
		mDescription = RuntimeManager.GetEventDescription(path);
		mDescription.isOneshot(out mIsOneShot);
	}

	/// <summary> FMOD에 설정된 파라미터를 변경합니다.</summary>
	/// <param name="name">FMOD에 셋팅되어있는 파라미터 이름</param>
	public void SetParmater(string name, float value)
	{
		FMOD.RESULT result = mDescription.getParameterDescriptionByName(name, out var parameDes);
		if (result == FMOD.RESULT.ERR_EVENT_NOTFOUND)
		{
			mDescription.getPath(out var path);
			Ulog.LogError(this, $"The [{path}] Sound is have not parameter : {name}");
			return;
		}

		if (mInstance.isValid())
		{
			mInstance.setParameterByName(name, value);
		}
	}

	#region Play

	public void Play(params SoundParameter[] parmeters)
	{
		createInstance();

		foreach (SoundParameter p in parmeters)
		{
			SetParmater(p.Name, p.Value);
		}

		mInstance.start();
	}

	/// <summary>
	/// 해당 위치에 사운드를 출력합니다.
	/// </summary>
	public void PlayToPosition(Vector2 position, params SoundParameter[] parmeters)
	{
		createInstance();

		foreach (SoundParameter p in parmeters)
		{
			SetParmater(p.Name, p.Value);
		}

		mInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
		mInstance.start();
	}

	/// <summary>
	/// 해당 게임오브젝트에 정보를 통해서 그 위치에 사운드를 출력합니다.
	/// </summary>
	/// <param name="isParameterReset">해당 사운드에 기본 파라미터 값으로 할 것인가</param>
	public void PlayToTarget(Transform target, Rigidbody2D rig2D = null, params SoundParameter[] parmeters)
	{
		if (!mInstance.isValid())
		{
			createInstance();
		}

		foreach (SoundParameter p in parmeters)
		{
			SetParmater(p.Name, p.Value);
		}

		RuntimeManager.AttachInstanceToGameObject(mInstance, target, rig2D);
		mInstance.start();
	}

	/// <summary> 공간 음향 사운드에 필요한 데이터를 업데이트 합니다.</summary>
	public void Update3DAttribute(Transform targetTransfrom, Rigidbody2D targetRig2D = null)
	{
		if (mInstance.isValid())
		{
			mInstance.set3DAttributes(RuntimeUtils.To3DAttributes(targetTransfrom, targetRig2D));
		}
	}

	#endregion

	public void Stop(FMOD.Studio.STOP_MODE mode)
	{
		if (mInstance.isValid())
		{
			mInstance.stop(mode);
		}
	}

	public void Pause()
	{
		if (mInstance.isValid())
		{
			mInstance.setPaused(true);
		}
	}

	public void Resume()
	{
		if (mInstance.isValid())
		{
			mInstance.setPaused(false);
		}
	}



	private void createInstance()
	{
		if (mInstance.isValid())
		{
			mInstance.release();
			mInstance.clearHandle();
		}

		mDescription.createInstance(out mInstance);
	}

	public void Clear()
	{
		if (mInstance.isValid())
		{
			mInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			mInstance.release();
			mInstance.clearHandle();
		}
	}

	public bool IsPlaying()
	{
		if (mInstance.isValid())
		{
			PLAYBACK_STATE playbackState;
			mInstance.getPlaybackState(out playbackState);
			return (playbackState != PLAYBACK_STATE.STOPPED);
		}
		return false;
	}
}

