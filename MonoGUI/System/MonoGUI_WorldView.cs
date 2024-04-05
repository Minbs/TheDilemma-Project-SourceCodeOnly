using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonoGUI
{
	public abstract class MonoGUI_WorldView : MonoGUI_View
	{
		private Transform mTargetTransfrom = null;
		private Action mOnClose;

		public Transform MoveStart;
		public Action MoveEnd;

		public void InitializeWorldView(Transform target, Action onClose)
		{
			mTargetTransfrom = target;
			mOnClose = onClose;
		}

	

		private void Update()
		{
			worldGUIUpdate();
		}

		private void worldGUIUpdate()
		{
			if (mTargetTransfrom == null)
				return;

			var screenPos = Camera.main.WorldToScreenPoint(mTargetTransfrom.position);
			Debug.Log(screenPos);
			ViewRectTransfrom.position = screenPos;
		}

		public void OnClose()
		{
			mOnClose?.Invoke();
		}
	}
}
