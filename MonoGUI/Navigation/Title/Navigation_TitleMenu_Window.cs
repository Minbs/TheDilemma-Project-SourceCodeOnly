using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaNet.Session;
using Utils;

namespace MonoGUI
{
	public class Navigation_TitleMenu_Window : MonoGUI_Navigation
	{
		private NetworkManageService mNetworkManageService;
		private bool mIsSteamValid => mNetworkManageService.IsSteamValid;

		public void Initialize()
		{
			mNetworkManageService = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();
		}


		/// <summary> 서버 탐색기 뷰를 보여줍니다.</summary>
		public void OpenServerBrowser()
		{
			var view = Switch<View_TitleMenu_ServerBrowser>();

			view.Initialized(() => 
			{
				Pop<View_TitleMenu_ServerBrowser>();
			});
		}

		/// <summary> 로비 생성 뷰를 보여줍니다.</summary>
		public void OpenCreateLobby()
		{
			if (!mIsSteamValid)
			{
				Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
				return;
			}

			var view = Switch<View_TitleMenu_CreateLobby>();
			view.Initialized(() =>
			{
				Pop<View_TitleMenu_CreateLobby>();
			});
		}

		/// <summary> 싱글플레이 뷰를 보여줍니다.</summary>
		public void OpenSinglePlay()
		{
			var view =Switch<View_TitleMenu_SinglePlay>();
			view.Initialized(() =>
			{
				Pop<View_TitleMenu_SinglePlay>();
			});
		}
	}
}
