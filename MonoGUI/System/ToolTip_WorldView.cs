using System;
using UnityEngine;

namespace MonoGUI
{
	public abstract class ToolTip_WorldView : ToolTip_View
	{
		protected Transform mTarget { set; get; }

		private void guiUpdate()
		{
			var screenPos = Camera.main.WorldToScreenPoint(mTarget.transform.position);
			ViewRectTransfrom.position = screenPos;
		}

		public void InitializeWorldToolTip(
			RectTransform drawRect, 
			Transform target, 
			bool isOverDraw,
			Action onClose)
		{
			InitializeToolTip(drawRect, isOverDraw);
			mTarget = target;
			mOnClose = onClose;
		}

		private void Update()
		{
			guiUpdate();
		}
	}
}
