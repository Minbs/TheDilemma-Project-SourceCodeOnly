using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonoGUI
{
	[Serializable]
	public class GuiSeqenceGroup
	{
		[SerializeField] private List<GUISeqenceElement> mTweenList;

		private Sequence mSequence = null;
		private Action mOnKill;
		private Action mOnComplete;
		private Action mOnStart;

		public GuiSeqenceGroup()
		{
			mSequence.OnStart(() =>
			{
				foreach (var element in mTweenList)
				{
					element.TweenAnimation.OnSequenceStart();
				}
				mOnStart?.Invoke();
			});

			mSequence.OnComplete(() =>
			{
				foreach (var element in mTweenList)
				{
					element.TweenAnimation.OnSequenceComplete();
				}
				mOnComplete?.Invoke();
			});

			mSequence.OnKill(() =>
			{
				mSequence = null;
				mOnKill?.Invoke();
			});
		}

		public void Initialize(MonoBehaviour mono)
		{
			if (mTweenList.Count > 0)
			{
				foreach (var element in mTweenList)
				{
					element.Initilze(mono);
				}
			}
		}

		public void Stop(Action callback = null)
		{
			// Bind callback
			mOnKill = callback;

			// Stop animation
			DOTween.Kill(mSequence);
		}

		public void Play(Action callback = null)
		{
			// Bind callback
			mOnComplete = callback;
			mOnKill = null;

			// Stop if it's playing
			DOTween.Kill(mSequence);

			// Create Sequence
			mSequence = DOTween.Sequence();

			foreach (var element in mTweenList)
			{
				if (element.SequenceType == GUISequenceType.Append)
					mSequence.Append(element.TweenAnimation.GetTween());
				else if (element.SequenceType == GUISequenceType.Join)
					mSequence.Join(element.TweenAnimation.GetTween());
			}

			// Play
			mSequence.Play();
		}
	}
}