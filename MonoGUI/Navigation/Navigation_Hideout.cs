using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaNet.Session;

namespace MonoGUI
{
	public class Navigation_Hideout : MonoGUI_Navigation
	{
		public void OnInitialized()
		{

		}

		public void OpenHideoutTerminal()
		{
			var view = Push<View_Terminal>();
			view.InitializeAsHideout(CloseHideoutTerminal);
		}

		public void CloseHideoutTerminal()
		{
			Pop<View_Terminal>();
		}
	}
}
