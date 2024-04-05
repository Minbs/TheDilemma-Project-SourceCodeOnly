using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
public class GUIAnimationRectScale : GUIAnimationBase
{
	private RectTransform mRectTransform;

	[Title("Rect Scale Option")]

	[SerializeField]
	private Vector2 StartScale;

	[SerializeField]
	private Vector2 EndScale;

	public override void Initilize()
	{
		base.Initilize();

		if (!TryGetComponent<RectTransform>(out mRectTransform))
		{
			Ulog.LogError(UlogType.UI, $"There is no RectTransform in {gameObject}");
			return;
		}

		IsAvailable = true;
	}
	public override Tween GetTween()
	{
		if (!IsAvailable)
		{
			Ulog.LogNoInitialize(this);
			return null;
		}

		Tween tween = mRectTransform.DOScale(EndScale, Duration);
		tween.SetLoops(loopCount, loopType);
		tween.SetDelay(Delay);

		tween.OnStart(() => OnTweenStart());
		tween.OnComplete(() => OnTweenComplete());

		return tween;
	}

	public override void OnSequenceStart()
	{
		base.OnSequenceStart();
		mRectTransform.localScale = StartScale;
	}

	public override void OnTweenComplete()
	{
		base.OnTweenComplete();
	}
}
