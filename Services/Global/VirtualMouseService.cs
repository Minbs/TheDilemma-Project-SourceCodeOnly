using Gameplay;
using MonoGUI;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Service;

public class VirtualMouseService : MonoService
{
	private Dictionary<AimType, View_Mouse> mAimTable = new();

	[SerializeField] private Navigation_Mouse mNavigation;
	private View_Mouse mCurrentMouse;

	public override void OnRegistered()
	{
		base.OnRegistered();

		Ulog.Log(this, "Virtual Mouse Registered");

		// Initialize mouse table
		View_MouseNormal mouseNormal = mNavigation.CreateMouseView<View_MouseNormal>();
		mAimTable.Add(AimType.Arrow, mouseNormal);

		View_MousePistol mousePistor = mNavigation.CreateMouseView<View_MousePistol>();
		mousePistor.Initiailzed();
		mAimTable.Add(AimType.Aim_Pistol, mousePistor);

		View_MouseShotgun mouseShotGun = mNavigation.CreateMouseView<View_MouseShotgun>();
		mouseShotGun.Initiailzed();
		mAimTable.Add(AimType.Aim_Shotgun, mouseShotGun);

		// Setup initial virtual mouse
		ChangeAim(AimType.Arrow);
		//Cursor.visible = false;
	}

	public override void OnUnregistered()
	{
		base.OnUnregistered();
	}

	private void Update()
	{
#if UNITY_EDITOR
		Cursor.visible = true;
#else
		Cursor.visible = false;
#endif
	}

	public void ChangeAim(AimType type)
	{
		mNavigation.ChangeMouse(type);
		mCurrentMouse = mAimTable[type];
		mCurrentMouse.MoveToRealMousePoint();
	}

	/// <summary> 해당 커서의 클릭 연출을 실행합니다.</summary>
	public void OnClick()
	{
		mCurrentMouse.OnClickAction();
	}

	/// <summary> 해당 커서의 재장전 연출을 실행합니다.</summary>
	public void OnReLoading()
	{
		mCurrentMouse.OnReload();
	}

	/// <summary>
	/// 현재 커서에 총기 반동을 GUI로 표현합니다.
	/// 최종 값은 0.0f ~ 1.0f 사의 값을 가지게됩니다.
	/// </summary>
	/// <param name="recoilPercent">추가할 반동 퍼센트</param>
	public void ApplyRecoil(float recoilPercent)
	{
		mCurrentMouse.ApplyRecoil(recoilPercent);
	}

	/// <summary> 가상 마우스 위치에서 Ray를 발사합니다.</summary>
	public Ray PointToRay()
	{
		return Camera.main.ScreenPointToRay(mCurrentMouse.GetScreenPosition()); ;
	}

	/// <summary> 가상 마우스의 월드 위치값을 가져옵니다.</summary>
	public Vector2 GetWorldPoint()
	{
		return Camera.main.ScreenToWorldPoint(mCurrentMouse.GetScreenPosition());
	}

	/// <summary>마우스 포인트와 접촉한 Collider들을 가져옵니다.</summary>
	public Collider2D[] GetOverlapColliders()
	{
		return Physics2D.OverlapPointAll(GetWorldPoint());
	}

	/// <summary>마우스 포인트와 접촉한 Collider들을 가져옵니다.</summary>
	public Collider2D[] GetOverlapColliders(int layerMask)
	{
		return Physics2D.OverlapPointAll(GetWorldPoint(), layerMask);
	}

	/// <summary>마우스 포인트와 접촉한 Collider를 가져옵니다.</summary>
	public Collider2D GetOverlapCollider()
	{
		return Physics2D.OverlapPoint(GetWorldPoint());
	}

	/// <summary>마우스 포인트와 접촉한 Collider를 가져옵니다.</summary>
	public Collider2D GetOverlapCollider(int layerMask)
	{
		return Physics2D.OverlapPoint(GetWorldPoint(), layerMask);
	}
}
