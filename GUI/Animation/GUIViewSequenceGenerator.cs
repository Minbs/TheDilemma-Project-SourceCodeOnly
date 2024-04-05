using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIViewSequenceGenerator : MonoBehaviour, IGUIAnimationControl
{
	[SerializeField] private List<GUISeqenceElement> mShowTweenList;
	[SerializeField] private List<GUISeqenceElement> mHideTweenList;

	public bool IsShowAnimatnioAvailable { private set; get; } = false;
	private bool mIsShowPlay = false;
	private Sequence mShowSequence = null;

	public bool IsHideAnimationAvailable { private set; get; } = false;
	private bool mIsHidePlay = false;
	private Sequence mHideSequence = null;

	private GUIView mView;

	private event Action mOnShowComplete;

	public event Action OnShowComplete
	{
		add
		{
			mOnShowComplete += value;
		}
		remove
		{
			mOnShowComplete -= value;
		}
	}

	private event Action mOnHideComplete;

	public event Action OnHideComplete
	{
		add
		{
			mOnHideComplete += value;
		}
		remove
		{
			mOnHideComplete -= value;
		}
	}

	public void Initialize(GUIView view)
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
			mView.gameObject.SetActive(true);

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
			mOnShowComplete?.Invoke();
			mView.State = VisableState.Appeared;
		});

		mShowSequence.OnKill(() =>
		{
			mIsShowPlay = false;
			mShowSequence = null;
		});

		mShowSequence.Play();
		mView.State = VisableState.Appearing;
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
			mOnHideComplete?.Invoke();
			callback?.Invoke();
			mView.gameObject.SetActive(false);
			mView.State = VisableState.Disappered;
		});

		mHideSequence.OnKill(() =>
		{
			mIsHidePlay = false;
			mHideSequence = null;
		});

		mHideSequence.Play();
		mView.State = VisableState.Disappearing;
		mIsHidePlay = true;
	}



}
