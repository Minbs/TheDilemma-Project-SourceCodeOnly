using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
public class View_WorldGUIView : MonoGUI_View
{
	[field: SerializeField]
	public Navigation_WorldGUI WorldGUINavigation { private set; get; }

	public override void OnInitialized()
	{
		
	}

	public void InitializeWorldGUI<T>(out MonoGUI_View view) where T : MonoGUI_WorldView
	{
		view =WorldGUINavigation.Push<T>();
	}

}
