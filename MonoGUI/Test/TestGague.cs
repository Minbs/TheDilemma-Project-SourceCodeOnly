using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;
public class TestGague : MonoBehaviour
{
	[SerializeField]
	private ImageViewModel Img_MainGague = new(nameof(Img_MainGague));
	[SerializeField]
	private ImageViewModel Img_ChangeGague = new(nameof(Img_ChangeGague));
	[SerializeField]
	private Gague test;

	private int mCurrent = 10;
	private int mMax = 10;

	private void Awake()
	{
		Img_MainGague.Initialize(this);
		Img_ChangeGague.Initialize(this);
		test = new(this,Img_MainGague, Img_ChangeGague, mCurrent, mMax, 0.25f);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.PageUp))
		{
			mCurrent++;
			mCurrent = Mathf.Clamp(mCurrent, 0, mMax);
			test.Increase(mCurrent, mMax);
		}
		else if (Input.GetKeyDown(KeyCode.PageDown))
		{
			mCurrent--;
			mCurrent = Mathf.Clamp(mCurrent, 0, mMax);
			test.Decrease(mCurrent, mMax);
		}

	}
}
