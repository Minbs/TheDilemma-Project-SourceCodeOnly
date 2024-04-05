using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;
using Utils;

public class Proxy_CreatureEntity : ProxyUnitBase
{
	protected Entity_Creature mEntity;

	[field: SerializeField] public Animator ProxyAnimator { get; set; }

	public void Initialize(Entity_Creature entity)
	{
		mEntity = entity;
	}

	public override void LookAt(Vector2 lookDirection)
	{
		Proxy.FlipByDirection(transform, lookDirection);
	}

	public override void PlayAnimation(AnimationType animationType)
	{
		if (animationType == AnimationType.None)
		{
			return;
		}

		ProxyAnimator.Play(animationType.GetAnimationName());
	}

	/// <summary>애니메이션 재생 시간을 반환합니다.</summary>
	/// <returns>해당 애니메이션이 없다면 false를 반환합니다.</returns>
	public override bool TryGetAnimationLength(AnimationType animationType, out float lengthSec)
	{
		var animatorController = ProxyAnimator.runtimeAnimatorController;
		var animationName = animationType.GetAnimationName();

		foreach (var clip in animatorController.animationClips)
		{
			if (clip.name == animationName)
			{
				lengthSec = clip.length;
				return true;
			}
		}

		lengthSec = -1;
		return false;
	}
}
