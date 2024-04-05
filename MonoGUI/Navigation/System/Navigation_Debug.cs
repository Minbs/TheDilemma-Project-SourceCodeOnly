using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using UnityEngine;
using Utils;

namespace MonoGUI
{
	public class Navigation_Debug : MonoGUI_Navigation
	{
		public bool IsOpen => mDebugView != null;
		private View_DebugLogPanel mDebugView;

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				if (IsOpen)
				{
					CloseDebug();
				}
				else
				{
					OpenDebug();
				}
			}
		}

		public void OpenDebug()
		{
			if (!IsOpen)
			{
				mDebugView = this.Push<View_DebugLogPanel>();
			}
		}

		public void CloseDebug()
		{
			if (IsOpen)
			{
				this.PopByObject(mDebugView.gameObject);
				mDebugView = null;
			}
		}
	}
}
