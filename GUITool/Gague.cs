using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Utils.ViewModel;
using DG.Tweening;
using System.Collections;

public enum GagueState
{
	None, 
	Increase, 
	Decrease
}


[Serializable]
public class Gague
{
	private StrachVerticalImage mMainStrachImage;
	private StrachVerticalImage mChangeStrachImage;

	private int mOldCurrent;
	private int mCurrent;

	private GagueState mState = GagueState.None;

	private MonoBehaviour mMono;

	private float mDecreaseSpeed = 1.0f;

	public Gague(MonoBehaviour mono, ImageViewModel mainImage, ImageViewModel changeImage, float decreaseSpeed)
		: this(mono, mainImage, changeImage, 50, 50, decreaseSpeed) {}

	public Gague
	(
		MonoBehaviour mono, 
		ImageViewModel mainImage, 
		ImageViewModel changeImage,
		int current, 
		int max, 
		float decreaseSpeed
	)
	{
		mMono = mono;
		mMainStrachImage = new(mainImage, 0.0f, 1.0f);
		mChangeStrachImage = new(changeImage, 0.0f, 0.0f);
		float parent = getParent(current, max);
		mainDraw(parent);
		mCurrent = current;
		mOldCurrent = mCurrent;
		mDecreaseSpeed = decreaseSpeed;
	}

	public void UpdateValue(int current, int max)
	{
		if (mCurrent <= current)
		{
			Increase(current, max);
		}
		else
		{
			Decrease(current, max);
		}
	}

	/// <summary> 게이지 감소 시</summary>
	public void Decrease(int current, int max)
	{
		mCurrent = current;
		float mainPercent = getParent(mCurrent, max);
		mainDraw(mainPercent);
		// 첫 감소
		if (mState != GagueState.Decrease)
		{
			float decrease = mOldCurrent - current;
			float decreasePercent = getParent(decrease, max);
			changeDraw(mainPercent, decreasePercent);
			mMono.StartCoroutine(decreaseAnimatnio());

			mState = GagueState.Decrease;
		}
		else
		{
			mChangeStrachImage.Min = mainPercent;
		}
	}
	/// <summary> 게이지 증가 시</summary>
	public void Increase(int current, int max)
	{
		mCurrent = current;
		float mainPercent = getParent(mCurrent, max);
		mainDraw(mainPercent);
		if(mState == GagueState.Decrease)
		{
			mChangeStrachImage.Min = mainPercent;
		}
		mOldCurrent = current;
	}


	//0.0 ~ 1.0 사이의 값만 넣으시요.
	private void mainDraw(float percent)
	{
		mMainStrachImage.Max = percent;
	}

	private void changeDraw(float mainPercent, float percent)
	{
		mChangeStrachImage.Min = mainPercent;
		mChangeStrachImage.Max = mainPercent + percent;
	}

	private float getParent(float current, float max)
	{
		return current / max;
	}

	private IEnumerator decreaseAnimatnio()
	{
		while (CanDecreaseAnimation())
		{
			yield return null;
			mChangeStrachImage.Max -= Time.deltaTime * mDecreaseSpeed;
		}
		mState = GagueState.None;
		mOldCurrent = mCurrent;
	}

	private bool CanDecreaseAnimation()
	{
		return mChangeStrachImage.Max - mChangeStrachImage.Min > 0.0f;
	}


}
