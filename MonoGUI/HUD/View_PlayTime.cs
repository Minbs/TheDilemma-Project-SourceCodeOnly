using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils;
using Utils.ViewModel;
public class View_PlayTime : MonoGUI_View
{
	[SerializeField] private TextMeshProTextViewModel Text_Playtime = new(nameof(Text_Playtime));

	public override void OnInitialized()
	{
		Text_Playtime.Initialize(this);
	}

	/// <summary> (구현 필요) 스테이지 시간을 출력합니다.</summary>
	public void DrawTime()
	{

	}
}
