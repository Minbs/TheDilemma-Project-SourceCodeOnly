using System;
using UnityEngine;
using Utils;

[Serializable]
public class GUISeqenceElement
{
	[SerializeField] private String ID;
	[field: SerializeField] public GUITweenType TweenType { private set; get; } = GUITweenType.None;
	[field: SerializeField] public GUISequenceType SequenceType { private set; get; } = GUISequenceType.None;

	private GUIAnimationBase mTweenAnimation;

	public GUIAnimationBase TweenAnimation { get => mTweenAnimation; }

	public void Initilze(MonoBehaviour mono)
	{
		if (!ID.IsValid())
		{
			Ulog.LogError(UlogType.UI, $"UI Tween ID is Null : {mono.gameObject.name}");
			return;
		}

		bool isFinded = false;

		#region Get UiAnimation

		if (TweenType == GUITweenType.None)
		{
			Ulog.LogError(UlogType.UI, $"Select Type is None");
			return;
		}
		else if (TweenType == GUITweenType.CanvasGroupFadeInout)
		{
			isFinded = TryGetUiAnimatino<GUIAnimationCanvasGrounpFadeInOut>(mono, out mTweenAnimation);
		}
		else if (TweenType == GUITweenType.RectMove)
		{
			isFinded = TryGetUiAnimatino<GUIAnimationRectMove>(mono, out mTweenAnimation);
		}
		else if (TweenType == GUITweenType.RectRotaion)
		{
			isFinded = TryGetUiAnimatino<GUIAnimationRectRotation>(mono, out mTweenAnimation);
		}
		else if (TweenType == GUITweenType.RectScale)
		{
			isFinded = TryGetUiAnimatino<GUIAnimationRectScale>(mono, out mTweenAnimation);
		}
		else if (TweenType == GUITweenType.ImageColor)
		{
			isFinded = TryGetUiAnimatino<GUIAnimationImageColor>(mono, out mTweenAnimation);
		}

		#endregion

		if (!isFinded)
		{
			Ulog.LogError(UlogType.UI, $"Initilze Failed. There is no {TweenType} component has that {ID}");
			return;
		}

		mTweenAnimation.Initilize();
	}

	private bool TryGetUiAnimatino<T>(MonoBehaviour mono, out GUIAnimationBase tweenAnimatino) where T : GUIAnimationBase
	{
		tweenAnimatino = null;

		#region Find component in mono

		var findedComponentsInMono = mono.GetComponents<T>();

		foreach (var findedMono in findedComponentsInMono)
		{
			if (findedMono.ID.EndsWith(ID))
			{
				tweenAnimatino = findedMono;
				return true;
			}
		}

		#endregion

		#region Find component in mono children

		var findedAnimatniosInChild = mono.GetComponentsInChildren<T>();
		foreach (var findedAnimationInChild in findedAnimatniosInChild)
		{
			if (findedAnimationInChild.ID.Equals(ID))
			{
				tweenAnimatino = findedAnimationInChild;
				return true;
			}
		}

		#endregion

		return false;
	}


}
