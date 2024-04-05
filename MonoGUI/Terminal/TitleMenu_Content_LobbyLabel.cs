using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.ViewModel;
using Utils;
using Sirenix.OdinInspector;
using Steamworks;
using System.Threading.Tasks;
using KaNet.SteamworksAPI;
using KaNet.Session;
using System;
using Steamworks.Data;

namespace MonoGUI
{
	public class TitleMenu_Content_LobbyLabel : MonoGUI_View
	{
		[SerializeField] private TextMeshProTextViewModel Text_Version = new(nameof(Text_Version));
		[SerializeField] private TextMeshProTextViewModel Text_LobbyName = new(nameof(Text_LobbyName));
		[SerializeField] private TextMeshProTextViewModel Text_PlayerCounter = new(nameof(Text_PlayerCounter));
		[SerializeField] private ButtonViewModel Btn_SelectLobby = new(nameof(Btn_SelectLobby));

		private Lobby? mLobbyInfo;
		private Action<EndPointInfo> mOnClick_JoinLobbyCallback;

		public bool IsSetup => mLobbyInfo != null;

		public override void OnInitialized()
		{
		}

		public void SetContextAsync(Lobby lobby, Action<EndPointInfo> onClickCallback)
		{
			Text_Version.Initialize(this);
			Text_LobbyName.Initialize(this);
			Text_PlayerCounter.Initialize(this);

			Btn_SelectLobby.Initialize(this);
			Btn_SelectLobby.BindAction(() =>
			{
				if (!IsSetup)
				{
					Ulog.Log(this, $"It hasn't set up lobby information.");
				}

				mOnClick_JoinLobbyCallback?.Invoke(new EndPointInfo(mLobbyInfo.Value));
			});

			mLobbyInfo = lobby;
			mOnClick_JoinLobbyCallback = onClickCallback;

			string version = lobby.GetGameVersion();
			Text_Version.Text = lobby.GetGameVersion();
			Text_LobbyName.Text = lobby.GetLobbyName();
			var maxMember = lobby.MaxMembers;
			var currentCount = lobby.MemberCount;

			Text_PlayerCounter.Text = $"{currentCount}/{maxMember}";
		}
	}
}
