using System;
using KaNet;
using KaNet.Session;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_TitleMenu_CreateLobby : MonoGUI_View
	{
		[SerializeField] TextMeshProInputFieldViewModel Input_LobbyName = new(nameof(Input_LobbyName));
		[SerializeField] TextMeshProInputFieldViewModel Input_LobbyDescription = new(nameof(Input_LobbyDescription));
		[SerializeField] ButtonViewModel Btn_CreateLobby = new(nameof(Btn_CreateLobby));
		[SerializeField] ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));
		 
		private Action<LobbySetting> mOnCreateLobby;

		private NetworkManageService mNetworkManageService;

		private Action mExitAction;

		public override void OnInitialized()
		{
			mNetworkManageService = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();

			Input_LobbyName.Initialize(this);
			Input_LobbyName.SetText(KaNetGlobal.DEFAULT_LOBBY_NAME);

			Input_LobbyDescription.Initialize(this);
			Input_LobbyDescription.SetText(KaNetGlobal.DEFAULT_LOBBY_DESCRIPTION);

			Btn_CreateLobby.Initialize(this);
			Btn_CreateLobby.BindAction(onCreateLobby);

			Btn_Exit.Initialize(this);
			Btn_Exit.BindAction(() =>
			{
				mExitAction?.Invoke();
			});

			mOnCreateLobby = mNetworkManageService.TryCreateLobby;
		}

		public void Initialized(Action exitAction)
		{
			mExitAction = exitAction;
		}

		private void onCreateLobby()
		{
			var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();
			NetOperationType callbackType = NetOperationType.CreateLobby;

			LobbySetting lobbySetting = new LobbySetting();

			lobbySetting.SetProgramID(ProcessHandler.Instance.ID);
			lobbySetting.SetLobbyName(Input_LobbyName.InputText);
			lobbySetting.SetLobbyDescription(Input_LobbyDescription.InputText);

			globalGui.ShowSystemDialog
			(
				callbackType,
				callbackType.GetTitle(),
				"로비 생성중...",
				(DialogResult.None, true)
			);

			mOnCreateLobby?.Invoke(lobbySetting);
			return;
		}
	}
}
