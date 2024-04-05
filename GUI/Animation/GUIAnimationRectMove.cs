using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

public class GUIAnimationRectMove : GUIAnimationBase
{
	[Title("Rect Move Option")]

	[VerticalGroup("StartAnchorPos")]
	[SerializeField]
	private Vector2 StartAnchorPos;

	[VerticalGroup("EndAnchorPos")]
	[SerializeField]
	private Vector2 EndAnchorPos;

	private RectTransform mRectTransform;

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

		Tween tween = mRectTransform.DOAnchorPos(EndAnchorPos, Duration);
		tween.SetLoops(loopCount, loopType);
		tween.SetDelay(Delay);
		tween.OnStart(() => OnTweenStart());
		tween.OnComplete(() => OnTweenComplete());

		return tween;
	}

	public override void OnSequenceComplete()
	{
		base.OnSequenceComplete();
		mRectTransform.anchoredPosition = EndAnchorPos;
	}

	public override void OnSequenceStart()
	{
		base.OnSequenceStart();
		mRectTransform.anchoredPosition = StartAnchorPos;
	}

	[Button]
	[VerticalGroup("StartAnchorPos")]
	public void SetStartPos()
	{
		RectTransform rectTransform = GetComponent<RectTransform>();
		StartAnchorPos = rectTransform.anchoredPosition;
	}

	[Button]
	[VerticalGroup("EndAnchorPos")]
	public void SetEndPos()
	{
		RectTransform rectTransform = GetComponent<RectTransform>();
		EndAnchorPos = rectTransform.anchoredPosition;
	}

}
