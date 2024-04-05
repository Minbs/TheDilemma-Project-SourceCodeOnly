using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Utils.ViewModel;


[Serializable]
public struct GunRenderer
{
	// Primary
	public Vector3 PrimaryWeaponPosition;
	public Vector3 MuzzlePosition;

	// Secondary
	public Vector3 SecondaryWeaponPosition;
	public Vector3 SecondaryWeaponRotation;

	// Common
	public Vector3 AimPosition;
	public Sprite GunSprite;
}

public class Equipment_Gun : MonoBehaviour
{
	private TransformViewModel Sprite_Weapon = new(nameof(Sprite_Weapon));
	private TransformViewModel Pivot_Muzzle = new(nameof(Pivot_Muzzle));
	private TransformViewModel Pivot_Aim = new(nameof(Pivot_Aim));

	public Transform AimTransform => Pivot_Aim.Transform;
	public Transform WeaponTransform => Sprite_Weapon.Transform;
	public Transform MuzzleTransform => Pivot_Muzzle.Transform;

	public GunRenderer GunRenderer;
	[field: SerializeField] public SpriteRenderer Weapon_Sprite { get; set; }
	
	private void Awake()
	{
		Weapon_Sprite = GetComponentInChildren<SpriteRenderer>();
		Sprite_Weapon.Initialize(this);
		Pivot_Muzzle.Initialize(this);
		Pivot_Aim.Initialize(this);
	}

	/// <summary>
	/// 대상 무기를 Primary Weapon으로 바꿉니다
	/// </summary>
	[Button]
	public void ChangePrimaryGun()
	{
		Sprite_Weapon.Transform.localPosition = GunRenderer.PrimaryWeaponPosition;
		Sprite_Weapon.Transform.localPosition = GunRenderer.PrimaryWeaponPosition;
		Sprite_Weapon.Transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		Pivot_Aim.Transform.localPosition = GunRenderer.AimPosition;

		Weapon_Sprite.sprite = GunRenderer.GunSprite;
	}

	/// <summary>
	/// 대상 무기를 Secondary Weapon으로 바꿉니다
	/// </summary>
	[Button]
	public void ChangeSecondaryGun()
	{
		Sprite_Weapon.Transform.localPosition = GunRenderer.SecondaryWeaponPosition;
		Sprite_Weapon.Transform.localEulerAngles = GunRenderer.SecondaryWeaponRotation;
		Pivot_Aim.Transform.localPosition = GunRenderer.AimPosition;

		Weapon_Sprite.sprite = GunRenderer.GunSprite;
	}

	public bool IsEqual(Equipment_Gun gun)
	{
		return false;
	}

#if UNITY_EDITOR

	private void OnValidate()
	{
		Sprite_Weapon.Initialize(this);
		Pivot_Muzzle.Initialize(this);
		Pivot_Aim.Initialize(this);

		GunRenderer.GunSprite = GetComponentInChildren<SpriteRenderer>().sprite;
	}

	[Button]
	public void SetPrimaryEquipmentInfo()
	{
		Sprite_Weapon.Initialize(this);
		Pivot_Muzzle.Initialize(this);
		Pivot_Aim.Initialize(this);

		GunRenderer.AimPosition = Pivot_Aim.Transform.localPosition;
		GunRenderer.PrimaryWeaponPosition = Sprite_Weapon.Transform.localPosition;
		GunRenderer.MuzzlePosition = Pivot_Muzzle.Transform.localPosition;
		GunRenderer.GunSprite = GetComponentInChildren<SpriteRenderer>().sprite;
	}

	[Button]
	public void SetSecondaryEquipmentInfo()
	{
		Sprite_Weapon.Initialize(this);
		Pivot_Muzzle.Initialize(this);
		Pivot_Aim.Initialize(this);

		GunRenderer.SecondaryWeaponPosition = Sprite_Weapon.Transform.localPosition;
		GunRenderer.SecondaryWeaponRotation = Sprite_Weapon.Transform.localEulerAngles;
		GunRenderer.GunSprite = GetComponentInChildren<SpriteRenderer>().sprite;
	}
#endif
}