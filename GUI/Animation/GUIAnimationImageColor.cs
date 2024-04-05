using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class GUIAnimationImageColor : GUIAnimationBase
{
	private Image mImage;

	[field: Header("Color Option")]
	[field: ColorUsage(true, false), SerializeField] private Color StartColor;
	[field: ColorUsage(true, false), SerializeField] private Color EndColor;

	public override void Initilize()
	{
		base.Initilize();

		if (!TryGetComponent<Image>(out mImage))
		{
			Ulog.LogNoComponent(this, mImage);
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

		Tween tween = mImage.DOColor(EndColor, Duration);
		tween.SetLoops(loopCount, loopType);
		tween.SetDelay(Delay);

		tween.OnStart(() => OnTweenStart());
		tween.OnComplete(() => OnTweenComplete());

		return tween;
	}
	public override void OnSequenceStart()
	{
		base.OnSequenceStart();
		mImage.color = StartColor;
	}

	public override void OnTweenComplete()
	{
		base.OnTweenComplete();
		mImage.color = EndColor;
	}

}
