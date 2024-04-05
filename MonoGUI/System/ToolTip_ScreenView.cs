using System;
using UnityEngine;

namespace MonoGUI
{
	public abstract class ToolTip_ScreenView : ToolTip_View
	{
		protected Vector2 mScreenPosition { set; get; }

		public void InitializeWorldToolTip(
			RectTransform drawRect,
			Vector2 screenPosition,
			bool isOverDraw,
			Action onClose)
		{
			InitializeToolTip(drawRect, isOverDraw);
			mScreenPosition = screenPosition;
			mOnClose = onClose;
			guiUpdate();
		}


		private void guiUpdate()
		{
			ViewRectTransfrom.position = mScreenPosition;
		}

		private void Update()
		{
			guiUpdate();
		}
	}
}
