using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MonoGUI;
public class Navigation_IngameHUD_ChatWindow : MonoGUI_Navigation
{
	public View_Chat ShowChat()
	{
		return Switch<View_Chat>();
	}

}
