using DG.Tweening;
using UnityEngine;
using Utils;

public class GUIAnimationCanvasGrounpFadeInOut : GUIAnimationBase
{
	private CanvasGroup mCanvasGroup;

	[field: Header("Fade Option"), SerializeField] private bool mIsFadeIn;

	private float mEndAlpha;
	private float mStartAlpha;

	public override void Initilize()
	{
		base.Initilize();

		if (!TryGetComponent<CanvasGroup>(out mCanvasGroup))
		{
			Ulog.LogNoComponent(this, mCanvasGroup);
			return;
		}

		#region Alpha Setting

		mStartAlpha = 0.0f;
		mEndAlpha = 1.0f;

		if (!mIsFadeIn)
		{
			mStartAlpha = 1.0f;
			mEndAlpha = 0.0f;
		}

		#endregion

		IsAvailable = true;
	}

	public override Tween GetTween()
	{
		if (!IsAvailable)
		{
			Ulog.LogNoInitialize(this);
			return null;
		}

		Tween tween = mCanvasGroup.DOFade(mEndAlpha, Duration);
		tween.SetLoops(loopCount, loopType);
		tween.SetDelay(Delay);
		tween.OnStart(() => OnTweenStart());
		tween.OnComplete(() => OnTweenComplete());

		return tween;
	}

	public override void OnSequenceStart()
	{
		base.OnSequenceStart();
		mCanvasGroup.alpha = mStartAlpha;
	}

	public override void OnTweenComplete()
	{
		base.OnTweenComplete();
		mCanvasGroup.alpha = mEndAlpha;
	}
}
