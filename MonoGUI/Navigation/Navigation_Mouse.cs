using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Gameplay;
public class Navigation_Mouse : MonoGUI_Navigation
{
	public void OnSwitchMouse<T>()where T : View_Mouse
	{
		Switch<T>();
	}


	public T CreateMouseView<T>()where T : View_Mouse
	{
		T mouseView = Switch<T>();
		return mouseView;
	}

	public void ChangeMouse(AimType type)
	{
		if (type == AimType.Arrow)
			Switch<View_MouseNormal>();
		else if (type == AimType.Aim_Pistol)
			Switch<View_MousePistol>();
		else if(type == AimType.Aim_Shotgun)
			Switch<View_MouseShotgun>();
	}

}
