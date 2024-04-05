using KaNet.Session;
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
	public class View_Terminal : MonoGUI_View
	{
		[Title("View Model")]
		// Buttons
		[SerializeField] private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));
		[SerializeField] private ButtonViewModel Btn_InviteFriends = new(nameof(Btn_InviteFriends));
		[SerializeField] private ButtonViewModel Btn_SinglePlay = new(nameof(Btn_SinglePlay));
		[SerializeField] private ButtonViewModel Btn_CreateLobby = new(nameof(Btn_CreateLobby));
		[SerializeField] private ButtonViewModel Btn_IpAddressConnect = new(nameof(Btn_IpAddressConnect));
		[SerializeField] private ButtonViewModel Btn_ServerBrowser = new(nameof(Btn_ServerBrowser));

		[SerializeField] private ImageViewModel Img_Wifi = new(nameof(Img_Wifi));
		[SerializeField] private TextMeshProTextViewModel Text_CurrentTime = new(nameof(Text_CurrentTime));

		[Title("Terminal Window")]
		[SerializeField] private Navigation_TerminalHideout_Window Navigation_HideoutTerminalWindow = null;

		[Title("Wifi Icon")]
		[SerializeField]
		private Sprite GoodConnection;
		[SerializeField]
		private Sprite BadConnection;

		// DI Field
		private NetworkManageService mNetworkManageService;

		private Action mOnCloseTerminal;

		public override void OnInitialized()
		{
			Img_Wifi.Initialize(this);
			Text_CurrentTime.Initialize(this);

			// Initialize Terminal Window

			var networkService = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();
			Navigation_HideoutTerminalWindow.Initialize(networkService);

			// Initialize View Models
			Btn_Exit.Initialize(this);
			Btn_Exit.BindAction(() => { mOnCloseTerminal?.Invoke(); });

			Btn_InviteFriends.Initialize(this);
			Btn_InviteFriends.BindAction(Navigation_HideoutTerminalWindow.OpenInvitedFriend);

			Btn_SinglePlay.Initialize(this);
			Btn_SinglePlay.BindAction(Navigation_HideoutTerminalWindow.OpenSinglePlay);

			Btn_CreateLobby.Initialize(this);
			Btn_CreateLobby.BindAction(Navigation_HideoutTerminalWindow.OpenCreateLobby);

			Btn_IpAddressConnect.Initialize(this);
			Btn_IpAddressConnect.BindAction(Navigation_HideoutTerminalWindow.OpenIpAddressconnect);

			Btn_ServerBrowser.Initialize(this);
			Btn_ServerBrowser.BindAction(Navigation_HideoutTerminalWindow.OpenServerBrowser);
		}

		public void InitializeAsHideout
		(
			Action onCloseButtonClicked
		)
		{
			mOnCloseTerminal = onCloseButtonClicked;
			mNetworkManageService = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();

			// If hideout
			Btn_Exit.SetActive(true);
			Btn_InviteFriends.SetActive(false);
			Btn_SinglePlay.SetActive(true);
			Btn_CreateLobby.SetActive(true);
			Btn_IpAddressConnect.SetActive(true);
			Btn_ServerBrowser.SetActive(true);

			if (!mNetworkManageService.IsSteamValid)
			{
				Btn_InviteFriends.SetInteractable(false);
				Btn_CreateLobby.SetInteractable(false);
				Btn_ServerBrowser.SetInteractable(false);
				Btn_ServerBrowser.SetInteractable(false);
			}

			// Set WIFI image
			Img_Wifi.Sprite = mNetworkManageService.IsSteamValid ? GoodConnection : BadConnection;

			StartCoroutine(onShowing());

			IEnumerator onShowing()
			{
				while (true)
				{
					Text_CurrentTime.Text = GlobalServiceLocator
						.SystemInformationService
						.GetServiceOrNull()
						.GetSystemTime();

					yield return new WaitForSecondsRealtime(60);
				}
			}
		}

		public void InitializeAsIngame
		(
			Action onCloseButtonClicked
		)
		{
			mOnCloseTerminal = onCloseButtonClicked;
			mNetworkManageService = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();

			// If hideout
			Btn_Exit.SetActive(true);
			Btn_InviteFriends.SetActive(false);
			Btn_SinglePlay.SetActive(false);
			Btn_CreateLobby.SetActive(false);
			Btn_IpAddressConnect.SetActive(false);
			Btn_ServerBrowser.SetActive(false);

			if (!mNetworkManageService.IsSteamValid)
			{
				Btn_InviteFriends.SetInteractable(false);
				Btn_CreateLobby.SetInteractable(false);
				Btn_ServerBrowser.SetInteractable(false);
				Btn_ServerBrowser.SetInteractable(false);
			}

			// Set WIFI image
			Img_Wifi.Sprite = mNetworkManageService.IsSteamValid ? GoodConnection : BadConnection;

			StartCoroutine(onShowing());

			IEnumerator onShowing()
			{
				while (true)
				{
					Text_CurrentTime.Text = GlobalServiceLocator
						.SystemInformationService
						.GetServiceOrNull()
						.GetSystemTime();

					yield return new WaitForSecondsRealtime(60);
				}
			}
		}

		//public void OpenInvitedFriend()
		//{
		//	if (!mIsSteamValid)
		//	{
		//		Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
		//		return;
		//	}

		//	if (!mNetworkManageService.TryGetFriendList(out var friends))
		//	{
		//		Ulog.Log(this, $"You cannot call \"{MethodBase.GetCurrentMethod().Name}\". The steam service is invalid!");
		//		return;
		//	}

		//	StartCoroutine(mMenu.ViewSwitching<Popup_TerminalInviteFriends>((view) =>
		//	{
		//		//view.Initialize(friends, mInGameGuiService.OnInviteFriend);
		//		view.Initialize(friends, mInGameGuiService.OnInviteFriend);
		//	}));
		//}
	}
}