using System;
using UnityEngine;
using Utils;

/// <summary>SceneManageService에서 관리 가능한 Mono객체입니다.</summary>
public abstract class MonoManageObject : MonoBehaviour, IManageable
{
	public event Action<MonoManageObject> OnRemovedByMono;

	public virtual void OnFinalize()
	{
		if (!mIsInitialized)
		{
			return;
		}

		mIsInitialized = false;
		Destroy(gameObject);
	}

	public virtual void OnInitialize()
	{
		mIsInitialized = true;
	}

	private bool mIsInitialized = false;

	protected virtual void OnDestroy()
	{
		if (mIsInitialized)
		{
			mIsInitialized = false;
			OnFinalize();
			OnRemovedByMono?.Invoke(this);
		}
	}
}
