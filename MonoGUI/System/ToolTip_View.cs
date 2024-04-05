using System;
using UnityEngine;

namespace MonoGUI
{
	public abstract class ToolTip_View : MonoGUI_View
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

		protected bool mIsOverDraw { get; private set; }

		protected Action mOnClose { get; set; }

		private RectTransform mDrawRectTransfrom;


		private void boundaryUpdate()
		{
			var pos = ViewRectTransfrom.anchoredPosition;

			var edgeToPivotLeft = ViewRectTransfrom.rect.width * ViewRectTransfrom.pivot.x;
			var edgeToPivotRight = ViewRectTransfrom.rect.width * (1.0f - ViewRectTransfrom.pivot.x);
			var edgeToPivotTop = ViewRectTransfrom.rect.height * (1.0f - ViewRectTransfrom.pivot.y);
			var edgoToPivotBottom = ViewRectTransfrom.rect.height * ViewRectTransfrom.pivot.y;

			var left = pos.x - edgeToPivotLeft;
			var right = pos.x + edgeToPivotRight;
			var top = pos.y + edgeToPivotTop;
			var bottom = pos.y - edgoToPivotBottom;


			Vector2 anchoredPos = ViewRectTransfrom.anchoredPosition;

			OverArea canvasOver = canvasOverCheck(left, right, top, bottom);

			if (canvasOver.HasFlag(OverArea.Left))
			{
				anchoredPos.x = edgeToPivotLeft;
			}
			else if (canvasOver.HasFlag(OverArea.Right))
			{
				anchoredPos.x = mDrawRectTransfrom.rect.width - edgeToPivotRight;
			}

			if (canvasOver.HasFlag(OverArea.Top))
			{
				anchoredPos.y = mDrawRectTransfrom.rect.height - edgeToPivotTop;
			}
			else if (canvasOver.HasFlag(OverArea.Bottom))
			{
				anchoredPos.y = edgoToPivotBottom;
			}

			ViewRectTransfrom.anchoredPosition = anchoredPos;
		}

		private OverArea canvasOverCheck(float left, float right, float top, float bottom)
		{
			OverArea overArea = OverArea.None;

			if (left < 0)
				overArea |= OverArea.Left;

			if (right > mDrawRectTransfrom.rect.width)
				overArea |= OverArea.Right;

			if (top > mDrawRectTransfrom.rect.height)
				overArea |= OverArea.Top;

			if (bottom < 0)
				overArea |= OverArea.Bottom;

			return overArea;
		}

		private void LateUpdate()
		{
			if (!mIsOverDraw)
				boundaryUpdate();
		}

		
		protected void InitializeToolTip(RectTransform drawRect, bool isOverDraw)
		{
			mDrawRectTransfrom = drawRect;
			mIsOverDraw = isOverDraw;

			if (!mIsOverDraw)
				boundaryUpdate();
		}

		public virtual void OnClose()
		{
			mOnClose?.Invoke();
		}
	}
}
