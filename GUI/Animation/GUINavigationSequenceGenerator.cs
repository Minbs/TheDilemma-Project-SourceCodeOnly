using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GUINavigationSequenceGenerator : MonoBehaviour, IGUIAnimationControl
{
	[SerializeField] private List<GUISeqenceElement> mShowTweenList;
	[SerializeField] private List<GUISeqenceElement> mHideTweenList;

	public bool IsShowAnimatnioAvailable { private set; get; } = false;
	private bool mIsShowPlay = false;
	private Sequence mShowSequence = null;

	public bool IsHideAnimationAvailable { private set; get; } = false;
	private bool mIsHidePlay = false;
	private Sequence mHideSequence = null;

	private GUINavigation mNavigation;

	public void Initialize(GUINavigation navigation)
	{
		mNavigation = navigation;

		#region Show Seqence Setting

		if (mShowTweenList.Count > 0)
		{
			foreach (var element in mShowTweenList)
			{
				element.Initilze(this);
			}

			IsShowAnimatnioAvailable = true;
		}

		#endregion

		#region Hide Seqence Setting

		if (mHideTweenList.Count > 0)
		{
			foreach (var element in mHideTweenList)
			{
				element.Initilze(this);
			}

			IsHideAnimationAvailable = true;
		}

		#endregion
	}

	private void CreateSequence(List<GUISeqenceElement> tweenList, out Sequence sequence)
	{
		sequence = DOTween.Sequence();

		foreach (var element in tweenList)
		{
			if (element.SequenceType == GUISequenceType.Append)
				sequence.Append(element.TweenAnimation.GetTween());

			else if (element.SequenceType == GUISequenceType.Join)
				sequence.Join(element.TweenAnimation.GetTween());
		}
	}

	public void PlayShow(Action callback = null)
	{
		if (!IsShowAnimatnioAvailable)
			return;

		if (mIsShowPlay)
		{
			mShowSequence.Kill();
		}

		CreateSequence(mShowTweenList, out mShowSequence);

		mShowSequence.OnStart(() =>
		{
			mNavigation.navigationEnable = true;
			foreach (var element in mShowTweenList)
			{
				element.TweenAnimation.OnSequenceStart();
			}
		});

		mShowSequence.OnComplete(() =>
		{
			foreach (var element in mShowTweenList)
			{
				element.TweenAnimation.OnSequenceComplete();
			}

			callback?.Invoke();
			mNavigation.State = VisableState.Appeared;
		});

		mShowSequence.OnKill(() =>
		{
			mIsShowPlay = false;
			mShowSequence = null;
		});

		mShowSequence.Play();
		mNavigation.State = VisableState.Appearing;
		mIsShowPlay = true;
	}

	public void PlayHide(Action callback = null)
	{
		if (!IsHideAnimationAvailable)
			return;

		if (mIsHidePlay)
		{
			DOTween.Kill(mHideSequence);
			mHideSequence = null;
		}

		CreateSequence(mHideTweenList, out mHideSequence);

		mHideSequence.OnStart(() =>
		{
			foreach (var element in mHideTweenList)
			{
				element.TweenAnimation.OnSequenceStart();
			}
		});

		mHideSequence.OnComplete(() =>
		{
			foreach (var element in mHideTweenList)
			{
				element.TweenAnimation.OnSequenceComplete();
			}

			callback?.Invoke();
			mNavigation.State = VisableState.Disappered;
			mNavigation.navigationEnable = false;
		});

		mHideSequence.OnKill(() =>
		{
			mIsHidePlay = false;
			mHideSequence = null;
		});

		mHideSequence.Play();
		mNavigation.State = VisableState.Disappearing;
		mIsHidePlay = true;
	}



}
