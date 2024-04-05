using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils.ViewModel;
public class View_ProgressLine : MonoGUI_View
{
	[SerializeField] private ImageViewModel Img_ProgessIcon = new(nameof(Img_ProgessIcon));
	private RectTransfromViewModel mRectTransfrom_ProgessIcon = new(nameof(Img_ProgessIcon));

	public override void OnInitialized()
	{
		Img_ProgessIcon.Initialize(this);
		mRectTransfrom_ProgessIcon.Initialize(this);
		Progress(0.0f);
	}

	/// <summary> 맵에 진행도를 보여줍니다.</summary>
	/// <param name="progress">진행도</param>
	public void Progress(float progress)
	{
		mRectTransfrom_ProgessIcon.SetMinAnchorX(progress);
		mRectTransfrom_ProgessIcon.SetMaxAnchorX(progress);
	}
}
