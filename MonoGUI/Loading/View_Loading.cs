using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils.ViewModel;
using System;
using Sirenix.OdinInspector;

public class View_Loading : MonoGUI_View
{
	[field: SerializeField] private TextMeshProTextViewModel Text_Loading = new(nameof(Text_Loading));

	[Title("Text Animation"),SerializeField] private int MaxDotAmount;
	[SerializeField] private float DotChangeTime;

	private IEnumerator mTextAnimation;

	private string mFixedText = "Loading";

	public override void OnInitialized()
	{
		Text_Loading.Initialize(this);

		OnStartShowing += animationReset;
		OnShowed += animationStart;
		OnStartHidding += animationStop;
	}

	private void animationReset()
	{
		animationStop();
		Text_Loading.Text = mFixedText;
	}

	private void animationStart()
	{
		mTextAnimation = textAnimation();
		StartCoroutine(mTextAnimation);
	}

	private void animationStop()
	{
		StopCoroutine(mTextAnimation);
	}

	private IEnumerator textAnimation()
	{
		int dotCount = 0;
		
		while(true)
		{
			if (dotCount > MaxDotAmount)
				dotCount = 0;

			string text = mFixedText;
			
			for(int i = 0; i< dotCount; i++)
			{
				text += ".";
			}
			Text_Loading.Text = text;
			dotCount++;
			yield return new WaitForSeconds(DotChangeTime);
		}
	}


}
