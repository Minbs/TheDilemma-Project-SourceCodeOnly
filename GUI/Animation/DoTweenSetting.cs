using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTweenSetting : MonoBehaviour
{
	private void Awake()
	{
		DOTween.Init();
		DOTween.defaultAutoPlay = AutoPlay.None;
	}
}
