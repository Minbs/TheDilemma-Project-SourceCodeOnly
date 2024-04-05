using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Synchronizers;
using UnityEngine;
using Utils;

public abstract class ProxyUnitBase : ProxyBase
{
	public abstract void PlayAnimation(AnimationType animationType);

	/// <summary>애니메이션 재생 시간을 반환합니다.</summary>
	/// <returns>해당 애니메이션이 없다면 false를 반환합니다.</returns>
	public abstract bool TryGetAnimationLength(AnimationType animationType, out float lengthSec);

	public abstract void LookAt(Vector2 lookDirection);
}
