using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NewGUIViewSequenceGenerator : MonoBehaviour, IGUIVisable
{
	[SerializeField] private List<GUISeqenceElement> mShowTweenList;
	[SerializeField] private List<GUISeqenceElement> mHideTweenList;

	public bool IsShowAnimatnioAvailable { private set; get; } = false;
	private bool mIsShowPlay = false;
	private Sequence mShowSequence = null;

	public bool IsHideAnimationAvailable { private set; get; } = false;
	private bool mIsHidePlay = false;
	private Sequence mHideSequence = null;

	private NewGUIView mView;

	public void Initialize(NewGUIView view)
	{
		mView = view;

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

	public void Show(Action callback = null)
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
			mView.gameObject.SetActive(true);

			foreach (var element in mShowTweenList)
			{
				element.TweenAnimation.OnSequenceStart();
			}
			mView.SetVisableStae(VisableState.Appearing);
		});

		mShowSequence.OnComplete(() =>
		{
			foreach (var element in mShowTweenList)
			{
				element.TweenAnimation.OnSequenceComplete();
			}
			callback?.Invoke();
			mView.SetVisableStae(VisableState.Appeared);
		});

		mShowSequence.OnKill(() =>
		{
			mIsShowPlay = false;
			mShowSequence = null;
		});

		mShowSequence.Play();
		mIsShowPlay = true;
	}

	public void Hide(Action callback = null)
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

			mView.SetVisableStae(VisableState.Disappearing);
		});

		mHideSequence.OnComplete(() =>
		{
			foreach (var element in mHideTweenList)
			{
				element.TweenAnimation.OnSequenceComplete();
			}

			callback?.Invoke();
			mView.SetVisableStae(VisableState.Disappered);
			mView.gameObject.SetActive(false);
		});

		mHideSequence.OnKill(() =>
		{
			mIsHidePlay = false;
			mHideSequence = null;
		});

		mHideSequence.Play();
		mIsHidePlay = true;
	}
}
