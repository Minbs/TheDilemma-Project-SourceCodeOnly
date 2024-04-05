using System;
using KaNet;
using KaNet.Session;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_Terminal_CreateLobby : MonoGUI_View
	{
		[SerializeField] TextMeshProInputFieldViewModel Input_LobbyName = new(nameof(Input_LobbyName));
		[SerializeField] TextMeshProInputFieldViewModel Input_LobbyDescription = new(nameof(Input_LobbyDescription));
		[SerializeField] TextMeshProInputFieldViewModel Input_LobbyMaxPlayer = new(nameof(Input_LobbyMaxPlayer));
		[SerializeField] TextMeshProInputFieldViewModel Input_LobbyPassword = new(nameof(Input_LobbyPassword));
		[SerializeField] ButtonViewModel Btn_CreateLobby = new(nameof(Btn_CreateLobby));

		private Action<LobbySetting> mOnCreateLobby;

		public override void OnInitialized()
		{
			Input_LobbyName.Initialize(this);
			Input_LobbyName.SetText(KaNetGlobal.DEFAULT_LOBBY_NAME);

			Input_LobbyDescription.Initialize(this);
			Input_LobbyDescription.SetText(KaNetGlobal.DEFAULT_LOBBY_DESCRIPTION);

			Input_LobbyMaxPlayer.Initialize(this);
			Input_LobbyMaxPlayer.SetText(KaNetGlobal.DEFAULT_MAX_PLAYER.ToString());

			Input_LobbyPassword.Initialize(this);
			Input_LobbyPassword.SetText("");

			Btn_CreateLobby.Initialize(this);
			Btn_CreateLobby.BindAction(onCreateLobby);
		}

		public void Initialize(Action<LobbySetting> onCreateLobby)
		{
			mOnCreateLobby = onCreateLobby;
		}

		private void onCreateLobby()
		{
			var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();
			NetOperationType callbackType = NetOperationType.CreateLobby;

			LobbySetting lobbySetting = new LobbySetting();

			if (!int.TryParse(Input_LobbyMaxPlayer.InputText, out var maxPlayer))
			{
				globalGui.ShowSystemDialog
				(
					callbackType,
					callbackType.GetTitle(),
					"잘못된 입력입니다.",
					(DialogResult.OK, true)
				);

				return;
			}

			lobbySetting.SetProgramID(ProcessHandler.Instance.ID);
			lobbySetting.SetLobbyName(Input_LobbyName.InputText);
			lobbySetting.SetLobbyDescription(Input_LobbyDescription.InputText);
			lobbySetting.SetLobbyPassword(Input_LobbyPassword.InputText);

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
