using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaNet.Session;
using KaNet.Synchronizers;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class Navigation_PlayerInfoGroup : MonoGUI_Navigation
	{
		private GameplayManager mGameplayManager;

		private BidirectionalMap<NetSessionID, View_TeamInfo> mTeamInfoTable = new();

		private bool mIsInitialize = false;

		public void Initialize(GameplayManager gameplayManager)
		{
			if (mIsInitialize)
			{
				return;
			}

			mIsInitialize = true;

			mGameplayManager = gameplayManager;

			mGameplayManager.IngameSessionHandler.OnSessionConnected += onSessionConnected;
			mGameplayManager.IngameSessionHandler.OnSessionDisconnected += onSessionDisconnected;

			// Initialize player info gruop
			var sessions = mGameplayManager.IngameSessionHandler.IngameSessions;

			foreach (var session in sessions)
			{
				onSessionConnected(session.ID);
			}
		}

		private void onSessionDisconnected(NetSessionID sessionID)
		{
			if (mTeamInfoTable.TryGetValue(sessionID, out var view))
			{
				this.PopByObject(view.gameObject);
			}
		}

		private void onSessionConnected(NetSessionID sessionID)
		{
			if (mGameplayManager.ClientID == sessionID)
			{
				return;
			}

			if (mGameplayManager
				.IngameSessionHandler
				.TryGetSessionInfoByID(sessionID, out var ingameSessionInfo))
			{
				var teamInfo = Push<View_TeamInfo>();
				teamInfo.Initialize(mGameplayManager, ingameSessionInfo);

				if (!mTeamInfoTable.TryAdd(ingameSessionInfo.ID, teamInfo))
				{
					Ulog.LogError(this, $"There is duplicated team session! {ingameSessionInfo}");
				}
			}
			else
			{
				Ulog.LogError(this, $"onSessionConnected Error! There is no session {sessionID}");
			}
		}
	}
}
