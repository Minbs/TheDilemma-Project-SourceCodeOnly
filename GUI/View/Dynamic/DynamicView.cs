using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Utils;

public class DynamicView : GUIView
{
	[Flags]
	private enum OverArea
	{
		None = 0,
		Left = 1 << 0,
		Right = 1 << 1,
		Bottom = 1 << 2,
		Top = 1 << 3,
	}

	private RectTransform mCanvasRect;

	public void Initialize(RectTransform canvasRect)
	{
		mCanvasRect = canvasRect;
		viewSetUp();
		Open();
	}

	public void InBoundarySetting(RectTransform rectTransfrom)
	{
		var pos = rectTransfrom.anchoredPosition;
		var halfWidth = rectTransfrom.rect.width * 0.5f;
		var halfHeight = rectTransfrom.rect.height * 0.5f;

		var left = pos.x - halfWidth;
		var right = pos.x + halfWidth;
		var top = pos.y + halfHeight;
		var bottom = pos.y - halfHeight;


		Vector2 anchoredPos = rectTransfrom.anchoredPosition;

		OverArea canvasOver = canvasOverCheck(left, right, top, bottom);

		if (canvasOver.HasFlag(OverArea.Left))
		{
			anchoredPos.x = halfWidth;
		}
		else if (canvasOver.HasFlag(OverArea.Right))
		{
			anchoredPos.x = mCanvasRect.rect.width - halfWidth;
		}

		if (canvasOver.HasFlag(OverArea.Top))
		{
			anchoredPos.y = mCanvasRect.rect.height - halfHeight;
		}
		else if (canvasOver.HasFlag(OverArea.Bottom))
		{
			anchoredPos.y = halfHeight;
		}

		rectTransfrom.anchoredPosition = anchoredPos;
	}

	private OverArea canvasOverCheck(float left, float right, float top, float bottom)
	{
		OverArea overArea = OverArea.None;

		if (left < 0)
			overArea |= OverArea.Left;

		if (right > mCanvasRect.rect.width)
			overArea |= OverArea.Right;

		if (top > mCanvasRect.rect.height)
			overArea |= OverArea.Top;

		if (bottom < 0)
			overArea |= OverArea.Bottom;

		return overArea;
	}

}
