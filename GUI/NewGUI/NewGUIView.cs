using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NewVisableState
{
	None,
	/// <summary> 보여지는 애니메이션 진행 중</summary>
	Appearing,
	/// <summary> 보여지는 애니메이션 완료</summary>
	Appeared,
	/// <summary> 사라지는 애니메이션 진행 중</summary>
	Disappearing,
	/// <summary> 사라지는 애니메이션 완료</summary>
	Disappered
}

public class NewGUIView : MonoBehaviour, IGUIVisable
{
	private VisableState mState;
	public VisableState State { get => mState; }

	private NewGUIViewSequenceGenerator mSequenceGenerator = null;

	public bool IsShow => State == VisableState.Appeared || State == VisableState.Appearing;

	public bool IsAnimation => mSequenceGenerator != null;


	public void SetVisableStae(VisableState state)
	{
		mState = state;
	}

	/// <summary> 해당 GUI에 애니메이션을 적용한다. </summary>
	/// <param name="sequenceGerator">적용할 애니메이션에 시퀀스</param>
	protected void SetAnimatnio(NewGUIViewSequenceGenerator sequenceGerator)
	{
		mSequenceGenerator = sequenceGerator;
		mSequenceGenerator.Initialize(this);
	}

	/// <summary> GUI 비활성화 한다. </summary>
	/// <param name="startCallback">Hide 애니메이션 시작 전 호출되는 Action</param>
	/// <param name="endCallback">Hide 애니메이션 끝나고 호출되는 Action</param>
	public void Hide(Action callback = null)
	{
		if (!IsShow)
			return;

		if(!IsAnimation)
		{
			callback?.Invoke();
			gameObject.SetActive(false);
		}

		mSequenceGenerator.Hide(callback);
	}

	/// <summary> GUI 활성화 한다. </summary>
	/// <param name="startCallback">Show 애니메이션 시작 전 호출되는 Action</param>
	/// <param name="endCallback">Show 애니메이션 끝나고 호출되는 Action</param>
	public void Show(Action callback = null)
	{
		if (IsShow)
			return;

		if (!IsAnimation)
		{
			callback?.Invoke();
			gameObject.SetActive(false);
		}
		mSequenceGenerator.Show(callback);
	}

	/// <summary> 단순 오브젝트 활성화 혹은 비활성화 기능 </summary>
	public void SetActive(bool isActive)
	{
		VisableState state = VisableState.Appeared;
		if (!isActive)
			state = VisableState.Disappered;

		gameObject.SetActive(isActive);
		SetVisableStae(state);
	}
}
