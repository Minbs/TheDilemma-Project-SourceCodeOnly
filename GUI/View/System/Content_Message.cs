using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Content_Message : GUIView
{
	private TextMeshProUGUI mText;

	private RectTransform mRectTransform;

	public float Hight
	{
		get
		{
			return mRectTransform.rect.height;
		}
	}

	public void Initialize(string msg)
	{
		viewSetUp();
		mText = GetComponent<TextMeshProUGUI>();

		mText.text = msg;
		Show();
	}
}
