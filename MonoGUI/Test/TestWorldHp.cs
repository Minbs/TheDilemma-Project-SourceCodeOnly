using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MonoGUI;
public class TestWorldHp : MonoBehaviour
{
	public Navigation_WorldGUI WorldGUI;
	public Transform mTarget;

	private View_WorldGUI_Hp mGuiHp;

	public Transform EndPoint;

	public void Start()
	{
		WorldGUI.OpenWorldGUI<View_WorldGUI_Hp>(mTarget, false, out mGuiHp);

		//transform.DOMove(EndPoint.position, 2.0f).SetLoops(-1, LoopType.Yoyo);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
			mGuiHp.OnClose();

		float sin = Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad));
		
		mGuiHp.Draw(sin);
	}


}
