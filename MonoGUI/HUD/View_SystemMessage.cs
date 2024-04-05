using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils;
using Utils.ViewModel;
using System;

public enum MessageType : byte
{
	Normal,
	//추가할 수 있는거
}

public class View_SystemMessage : MonoGUI_View
{
	[SerializeField] private TextMeshProTextViewModel Text_Message = new(nameof(Text_Message));
	private IEnumerator mDuration;

	public override void OnInitialized()
	{
		Ulog.Log($"[HUD] Draw SystemMessage");
		Text_Message.Initialize(this);
	}

	/// <summary> 시스템 메세지를 출력합니다. </summary>
	/// <param name="type">출력 효과 타입</param>
	/// <param name="msg">메세지</param>
	/// <param name="time">유지시간은 Show연출과 Hide연출을 제외한 시간입니다.</param>
	public void Message(MessageType type, string msg, float time)
	{
		if (mDuration != null)
		{
			StopCoroutine(mDuration);
		}

		if (type == MessageType.Normal)
		{
			Text_Message.Text = msg;
			Show();
		}

		mDuration = duration(time);
		StartCoroutine(mDuration);
	}

	private IEnumerator duration(float t)
	{
		//보이는 연출이 끝날때 까지 대기 합니다.
		while(CurrentState == GuiState.Showing)
		{
			yield return null;
		}	
		//보이는 연출 이후부터 지속시간만큼 대기합니다.
		yield return new WaitForSeconds(t);
		Hide();
	}

	public void Close()
	{
		if (mDuration != null)
		{
			StopCoroutine(mDuration);
			mDuration = null;
		}

		ShowSkip();
		Hide();
		HideSkip();
	}

}
