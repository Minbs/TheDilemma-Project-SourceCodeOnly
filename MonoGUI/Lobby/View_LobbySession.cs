using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.SteamworksAPI;
using KaNet.Synchronizers;
using UnityEditor;
using UnityEngine;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_LobbySession : MonoGUI_View
	{
		[SerializeField] private TextMeshProTextViewModel Text_PlayerName = new(nameof(Text_PlayerName));
		[SerializeField] private TextMeshProTextViewModel Text_IsReady = new(nameof(Text_IsReady));
		[SerializeField] private RawImageViewModel Img_SteamAvatar = new(nameof(Img_SteamAvatar));
		[SerializeField] private ImageViewModel Img_CharacterAvatarProfile = new(nameof(Img_CharacterAvatarProfile));

		public IngameSessionInfo SessionInfo { get; private set; }

		public override void OnInitialized()
		{
			Text_PlayerName.Initialize(this);
			Text_IsReady.Initialize(this);
			Img_SteamAvatar.Initialize(this);
			Img_CharacterAvatarProfile.Initialize(this);
		}

		public void UpdateBySession(IngameSessionInfo session)
		{
			SessionInfo = session;

			// TODO set character icon
			switch (SessionInfo.Character.GetEnum())
			{
				case Gameplay.CharacterType.Soldier:
					Img_CharacterAvatarProfile.Color = new Color(1f, 0.0f, 0.0f);
					break;

				case Gameplay.CharacterType.Sniper:
					Img_CharacterAvatarProfile.Color = new Color(0.0f, 0.0f, 0.0f);
					break;

				case Gameplay.CharacterType.Police:
					Img_CharacterAvatarProfile.Color = new Color(0.0f, 0.0f, 1f);
					break;

				case Gameplay.CharacterType.Engineer:
					Img_CharacterAvatarProfile.Color = new Color(1f, 1f, 0.0f);
					break;

				default:
					Img_CharacterAvatarProfile.Color = new Color(0.4f, 0.4f, 0.4f);
					break;
			}

			Text_PlayerName.Text = SessionInfo.Name;
			Text_IsReady.Text = SessionInfo.IsReadyToPlay ? "Ready" : "Not Ready";
		}

		public async Task Initialize(IngameSessionInfo session)
		{
			UpdateBySession(session);

			Img_SteamAvatar.Texture = await SteamUtilExtension
				.GetTextureFromSteamIDAsync(SessionInfo.SteamID.Value);
		}
	}
}
