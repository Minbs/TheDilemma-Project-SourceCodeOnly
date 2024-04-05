using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Session;

namespace MonoGUI
{

	public class Navigation_Ingame : MonoGUI_Navigation
	{
		public void OnInitialized()
		{

		}

		public void SwitchToLobby(IngameSessionHandler sessionHandler)
		{
			Switch<View_Lobby>().Initialize(sessionHandler);
		}

		public void SwitchToAlive(GameplayManager gameplayManager)
		{
			var hud = Switch<View_IngameAlive>();
			hud.InitializeByManager(gameplayManager);
		}

		public void ShowSystemMessage(MessageType messageType, string message)
		{
			if (TryFind<View_IngameAlive>(out var view))
			{
				view.DrawSystemMessage(messageType, message, 3.0f);
			}
		}

		public void OpenIngameTerminal()
		{
			var view = Push<View_Terminal>();
			view.InitializeAsIngame(CloseIngameTerminal);
		}

		public void CloseIngameTerminal()
		{
			Pop<View_Terminal>();
		}

		public void OpenInGameHUD()
		{
			Push<View_IngameAlive>();
		}
	}
}
