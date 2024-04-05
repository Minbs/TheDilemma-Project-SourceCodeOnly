using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils.ViewModel;

public class StrachVerticalImage
{
	private float mMin;
	private float mMax;

	private ImageViewModel mImage;
	public RectTransform ImageRectTransfrom { private set; get; }

	public bool IsRunAnimation;

	public float Min
	{
		set
		{
			mMin = Mathf.Clamp(value, 0.0f, 1.0f);
			minUpdate();
		}
		get
		{
			return mMin;
		}
	}

	public float Max
	{
		set
		{
			mMax = Mathf.Clamp(value, 0.0f, 1.0f);
			maxUpdate();
		}
		get
		{
			return mMax;
		}
	}

	public StrachVerticalImage(ImageViewModel image, float min, float max)
	{
		mImage = image;

		ImageRectTransfrom = mImage.GetViewGameObject().GetComponent<RectTransform>();
		ImageRectTransfrom.anchoredPosition = Vector2.zero;
		ImageRectTransfrom.anchorMin = Vector2.zero;
		ImageRectTransfrom.anchorMax = Vector2.one;
		ImageRectTransfrom.sizeDelta = Vector2.zero;

		Min = min;
		Max = max;
	}


	private void minUpdate()
	{
		Vector2 anchorMin = ImageRectTransfrom.anchorMin;
		anchorMin.x = Min;

		ImageRectTransfrom.anchorMin = anchorMin;
		ImageRectTransfrom.offsetMin = new Vector2(0.0f, ImageRectTransfrom.offsetMin.y);
	}

	private void maxUpdate()
	{
		Vector2 anchorMax = ImageRectTransfrom.anchorMax;
		anchorMax.x = Max;

		ImageRectTransfrom.anchorMax = anchorMax;
		ImageRectTransfrom.offsetMax = new Vector2(0.0f, ImageRectTransfrom.offsetMax.y);
	}

	public void SetActive(bool isActive)
	{
		mImage.GetViewGameObject().SetActive(isActive);
	}
}
