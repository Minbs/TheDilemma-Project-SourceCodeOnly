using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;

public class Content_UserLobbyPanel : GUIView
{


	[SerializeField] private ButtonViewModel Btn_Select = new(nameof(Btn_Select));
	[SerializeField] private TextViewModel Text_Username = new(nameof(Text_Username));
	[SerializeField] private RawImageViewModel Img_Avatar = new(nameof(Img_Avatar));

	public void Initialize()
	{

	}
}
