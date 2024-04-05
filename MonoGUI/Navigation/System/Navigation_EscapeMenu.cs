using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaNet.Session;

namespace MonoGUI
{
	public struct EscapeButtonInfo
	{
		public string ButtonName;
		public Action OnClick;
		public bool IsQuickMenu;

		public EscapeButtonInfo(string buttonName, Action onClick, bool isQuickMenu)
		{
			ButtonName = buttonName;
			OnClick = onClick;
			IsQuickMenu = isQuickMenu;
		}
	}

	public class Navigation_EscapeMenu : MonoGUI_Navigation
	{
		private List<EscapeButtonInfo> mEscapeButtonInfos = new();

		public void Initialize()
		{

		}

		public void SetupButtons(IList<EscapeButtonInfo> buttonInfos)
		{
			mEscapeButtonInfos = new List<EscapeButtonInfo>(buttonInfos);
		}

		public void OpenEscapeMenu()
		{
			var view = Push<View_Escape>();
			view.Initialize(mEscapeButtonInfos);
		}

		public void OpenOption()
		{
			var view = Push<View_Option>();
			view.Initialize(()=> Pop());
		}

		public void CloseEscapeMenu()
		{
			Pop();
		}

		public void BindButtonInfos(IList<EscapeButtonInfo> buttonInfos)
		{
			mEscapeButtonInfos.Clear();
			mEscapeButtonInfos.AddRange(buttonInfos);
		}
	}
}
