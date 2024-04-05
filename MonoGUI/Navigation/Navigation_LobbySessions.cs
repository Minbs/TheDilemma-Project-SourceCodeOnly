using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gameplay;
using KaNet.Session;
using KaNet.Synchronizers;

namespace MonoGUI
{
	public class Navigation_LobbySessions : MonoGUI_Navigation
	{
		private Dictionary<NetSessionID, View_LobbySession> mSessionTable = new();

		public void Setup(IngameSessionHandler handler)
		{
			var sessions = handler.IngameSessions;

			foreach (var s in sessions)
			{
				// 이미 존재하는 객체는 업데이트
				if (mSessionTable.TryGetValue(s.ID, out var lobbySession))
				{
					lobbySession.UpdateBySession(s);
					continue;
				}

				// 새로운 객체는 추가
				var newSession = Push<View_LobbySession>();
				mSessionTable.Add(s.ID, newSession);
				var t = newSession.Initialize(s);
			}

			foreach (var id in mSessionTable.Keys.ToArray())
			{
				bool isExist = false;

				foreach (var s in sessions)
				{
					if (s.ID == id)
					{
						isExist = true;
						break;
					}
				}

				if (!isExist)
				{
					var removeView = mSessionTable[id];
					mSessionTable.Remove(id);
					PopByObject(removeView.gameObject);
				}
			}
		}
	}
}
