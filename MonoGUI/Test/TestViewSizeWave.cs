using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestViewSizeWave : MonoBehaviour
{
	[SerializeField]
	private RectTransform mRectTransfrom;

	public float Frequency;

	public float Magnitude;

	private void Update()
	{
		float sin = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup) * Frequency) * Magnitude + 50.0f;
		mRectTransfrom.sizeDelta = new Vector2(sin, sin);
	}
}
