using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;

public class GuiTestScript : MonoBehaviour
{
	public MonoGUI_Navigation Navigation;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Navigation.Push<View_Test1>();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Navigation.Push<View_Test2>();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Navigation.Push<View_Test3>();
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Navigation.Switch<View_Test1>();
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			Navigation.Switch<View_Test2>();
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			Navigation.Switch<View_Test3>();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Navigation.Pop();
		}
	}
}
