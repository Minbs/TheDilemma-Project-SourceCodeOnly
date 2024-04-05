using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Utils;
using Utils.ViewModel;
using KaNet.Synchronizers;
using Gameplay;

public class Proxy_EntityPlayer : ProxyUnitBase
{
	[field: SerializeField] public Animator ProxyAnimator { get; set; }

	private TransformViewModel Pivot_Aim = new(nameof(Pivot_Aim));
	private TransformViewModel Pivot_Muzzle = new(nameof(Pivot_Muzzle));
	private TransformViewModel Pivot_SecondaryWeapon = new(nameof(Pivot_SecondaryWeapon));
	public Transform AimTransform => Pivot_Aim.Transform;
	public Transform MuzzleTransform => Pivot_Muzzle.Transform;
	public Transform SecondaryWeaponTransform => Pivot_SecondaryWeapon.Transform;

	public Vector2 LookDirection { get; private set; }
	[field: SerializeField] public float LookRotationSpeed { get; set; } = 0.2f;
	public AnimationType CurrentAnimation { get; private set; }
	private float mMoveTimestamp;
	public bool HasMoveInput => mMoveTimestamp > 0;
	public bool LookUp = false;
	
	[field: SerializeField] public SerializableDictionary<CharacterType, RuntimeAnimatorController> CharacterAnimationController { get; set; } = new();

	void Awake()
	{
		Pivot_Aim.Initialize(this);
		Pivot_Muzzle.Initialize(this);
		Pivot_SecondaryWeapon.Initialize(this);
	}

	public void Initialize()
	{
		CurrentAnimation = AnimationType.Idle_Front;
	}

	public void SetCharacterType(CharacterType characterType)
	{
		if (characterType == CharacterType.None)
		{
			characterType = CharacterType.Police;
		}

		if (!CharacterAnimationController.TryGetValue(characterType, out RuntimeAnimatorController controller))
		{
			Ulog.LogError(this, "애니메이터 컨트롤러 찾을 수 없음");
		}

		ProxyAnimator.runtimeAnimatorController = controller;
	}

	public override void OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		base.OnUpdate(deltaTimeInfo);

		// Check if currently moving
		if (mMoveTimestamp > 0)
		{
			mMoveTimestamp -= deltaTimeInfo.ScaledDeltaTime;
		}

		// TODO : 무기 바라보는 방향에 따라서 뎁스 조정
		// TODO : 보조 무기를 등에 들게 하기
		if (transform.hasChanged)
		{
			int sortOffset = LookUp ? -1 : 1;
			AimTransform.GetComponentInChildren<Renderer>().sortingOrder = Global.RoundByDepthOrderOffset(transform) + sortOffset;
			SecondaryWeaponTransform.GetComponentInChildren<Renderer>().sortingOrder = Global.RoundByDepthOrderOffset(transform) - sortOffset;
		}

		// When player look up
		if (LookUp)
		{
			CurrentAnimation = HasMoveInput ?
				AnimationType.Run_Back : AnimationType.Idle_Back;
		}
		else
		{
			CurrentAnimation = HasMoveInput ?
				AnimationType.Run_Front : AnimationType.Idle_Front;
		}
	}

	public override void LookAt(Vector2 lookDirection)
	{
		LookDirection = Vector2.Lerp(LookDirection, lookDirection, LookRotationSpeed); // deltaTime 으로 수정하기
		Proxy.FlipByDirection(transform, LookDirection);

		// Set look direction
		float lookFactor = Vector2.Dot(LookDirection, Vector2.up);
		if (LookUp && lookFactor < 0)
		{
			LookUp = false;
		}
		else if (lookFactor > 0.4)
		{
			LookUp = true;
		}

		// Set weapon aim direction
		float aimFactor = Vector2.Dot(LookDirection, Vector2.right);
		float scaleFactor = (aimFactor < 0) ? -1 : 1;
		AimTransform.right = LookDirection;
		AimTransform.localScale = new Vector3(scaleFactor, scaleFactor);
	}

	public void Move(float inputDeltaTime)
	{
		mMoveTimestamp = inputDeltaTime * 6;
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
