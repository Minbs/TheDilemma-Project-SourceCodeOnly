using KaNet.Session;
using KaNet.SteamworksAPI;
using KaNet.Synchronizers;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class Navigation_TerminalHideout_Window : MonoGUI_Navigation
	{
		// DI Field
		private NetworkManageService mNetworkManageService;
		private bool mIsSteamValid => mNetworkManageService.IsSteamValid;

		public void Initialize(NetworkManageService networkManageService)
		{
			mNetworkManageService = networkManageService;
		}

		#region View Open Events

		public void OpenInvitedFriend()
		{
			if (!mIsSteamValid)
			{
				Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
				return;
			}

			if (!mNetworkManageService.TryGetFriendList(out var friends))
			{
				Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
				return;
			}

			var view = this.Switch<View_Terminal_InviteFriends>();
			view.Initialize(friends, mNetworkManageService.TryInviteFriend);
		}

		public void OpenCreateLobby()
		{
			if (!mIsSteamValid)
			{
				Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
				return;
			}

			var view = this.Switch<View_Terminal_CreateLobby>();
			view.Initialize(mNetworkManageService.TryCreateLobby);
		}

		public void OpenServerBrowser()
		{
			if (!mIsSteamValid)
			{
				Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
				return;
			}

			var view = this.Switch<View_Terminal_ServerBrowser>();
			view.Initialize(() => 
			{
				mNetworkManageService.RequestLobbyList((lobbys) =>
				{
					// 접속 시도를 합니다.
					view.RefreshLobbyList(lobbys, (endpoint) =>
					{
						var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();

						if (endpoint.TargetLobby.HasPassword())
						{
							var passwordDialog = globalGui.ShowSystemDialog
							(
								NetOperationType.TryConnect,
								NetOperationType.TryConnect.GetTitle(),
								"비밀번호를 입력하세요.<구현중>",
								(DialogResult.OK, true),
								(DialogResult.Cancel, true)
							);

							passwordDialog.OnResultCallback += (result) =>
							{
								if (result == DialogResult.OK)
								{
									tryConnect(endpoint, new NetConnectRequestInfo
									(
										ProcessHandler.Instance.ID,
										"" // Password
									)); 
								}
							};

							return;
						}

						tryConnect(endpoint, new NetConnectRequestInfo
						(
							ProcessHandler.Instance.ID,
							"" // Password
						));
					});
				});
			});

			void tryConnect(EndPointInfo endPointInfo, NetConnectRequestInfo requestInfo)
			{
				var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();

				globalGui.ShowSystemDialog
				(
					NetOperationType.TryConnect,
					NetOperationType.TryConnect.GetTitle(),
					"접속중...",
					(DialogResult.None, false)
				);

				mNetworkManageService.TryConnectTo(endPointInfo, requestInfo);
			}
		}

		public void OpenSinglePlay()
		{
			var view = this.Switch<View_Terminal_SinglePlay>();
		}

		public void OpenIpAddressconnect()
		{
			var view = this.Switch<View_Terminal_IpAddressConnect>();
		}

		#endregion
	}
}