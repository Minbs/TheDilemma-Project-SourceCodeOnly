using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
public class Navigation_TestGUIAnimation : MonoGUI_Navigation
{

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
			Push<View_TestGUIAni>();
		if (Input.GetKeyDown(KeyCode.Alpha2))
			Pop<View_TestGUIAni>();
	}
}
