using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CharacterAnimation : MonoBehaviour
{
	[field: SerializeField] public Animator CharacterAnimator { get; set; }
	private void OnValidate()
	{
		CharacterAnimator = GetComponent<Animator>();
	}

	public void PlayAnimation(string animationName, bool isPlayBack = true)
	{
		if(isPlayBack)
		{
			CharacterAnimator.Play(animationName);
		}
		else
		{
			CharacterAnimator.Play(animationName, -1, 0);
		}
	}

	public void PlaySequenceAnimation(params string[] animationsName)
	{
		
	}

	public bool isPlayingAnimation(string animationName)
	{
		if(CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
		{
			return true;	
		}

		return false;
	}

	public bool isPlayingAnimation()
	{
		if(CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
		{
			return true;
		}

		return false;
	}

	public void SetAnimationSpeed(float speed)
	{
		CharacterAnimator.speed = speed;
	}
}