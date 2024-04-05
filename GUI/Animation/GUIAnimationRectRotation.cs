using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

public class GUIAnimationRectRotation : GUIAnimationBase
{
	private RectTransform mRectTransform;

	[Title("Rect Roation Option")]

	[SerializeField]
	private Vector3 StartAngle;

	[SerializeField]
	private Vector3 EndAngle;

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

		Tween tween = mRectTransform.DORotate(EndAngle, Duration);
		tween.SetLoops(loopCount, loopType);
		tween.SetDelay(Delay);
		tween.OnStart(() => OnTweenStart());
		tween.OnComplete(() => OnTweenComplete());

		return tween;
	}

	public override void OnSequenceStart()
	{
		base.OnSequenceStart();
		mRectTransform.rotation = Quaternion.Euler(StartAngle);
	}
}
