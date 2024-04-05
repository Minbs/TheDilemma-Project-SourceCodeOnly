using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using KaNet.SteamworksAPI;
using Steamworks.Data;
using UnityEngine;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_Lobby : MonoGUI_View
	{
		[SerializeField] private TextMeshProTextViewModel Text_LobbyName = new(nameof(Text_LobbyName));
		[SerializeField] private ButtonViewModel Btn_ReadyOrStart = new(nameof(Btn_ReadyOrStart));
		[SerializeField] private TextMeshProTextViewModel Text_ReadyOrStart = new(nameof(Text_ReadyOrStart));

		[SerializeField] private Navigation_LobbySessions Nav_LobbySessions;

		// Character Selector
		[SerializeField] private ButtonViewModel Btn_Selector_Soldier = new(nameof(Btn_Selector_Soldier));
		[SerializeField] private ButtonViewModel Btn_Selector_Police = new(nameof(Btn_Selector_Police));
		[SerializeField] private ButtonViewModel Btn_Selector_Sniper = new(nameof(Btn_Selector_Sniper));
		[SerializeField] private ButtonViewModel Btn_Selector_Engineer = new(nameof(Btn_Selector_Engineer));

		private Dictionary<CharacterType, ButtonViewModel> mCharacterSelectorTable = new();

		public override void OnInitialized()
		{
			Text_LobbyName.Initialize(this);

			Btn_ReadyOrStart.Initialize(this);
			Text_ReadyOrStart.Initialize(this);

			Btn_Selector_Soldier.Initialize(this);
			Btn_Selector_Police.Initialize(this);
			Btn_Selector_Sniper.Initialize(this);
			Btn_Selector_Engineer.Initialize(this);

			mCharacterSelectorTable.Add(CharacterType.Soldier, Btn_Selector_Soldier);
			mCharacterSelectorTable.Add(CharacterType.Police, Btn_Selector_Police);
			mCharacterSelectorTable.Add(CharacterType.Sniper, Btn_Selector_Sniper);
			mCharacterSelectorTable.Add(CharacterType.Engineer, Btn_Selector_Engineer);
		}

		private IngameSessionHandler mSessionHandler = null;

		public void Initialize(IngameSessionHandler sessionHandler)
		{
			Btn_Selector_Soldier.BindAction(() => 
			{
				sessionHandler.OnSelectCharacter(CharacterType.Soldier); 
			});
			Btn_Selector_Police.BindAction(() =>
			{
				sessionHandler.OnSelectCharacter(CharacterType.Police);
			});
			Btn_Selector_Sniper.BindAction(() =>
			{
				sessionHandler.OnSelectCharacter(CharacterType.Sniper);
			});
			Btn_Selector_Engineer.BindAction(() =>
			{
				sessionHandler.OnSelectCharacter(CharacterType.Engineer);
			});

			// Setup initial state
			mSessionHandler = sessionHandler;
			mSessionHandler.OnSessionChanged += onSessionChanged;
			this.OnStartHidding += () => { mSessionHandler.OnSessionChanged -= onSessionChanged; };

			onSessionChanged(sessionHandler);

			// Start refresh steam lobby
			StartCoroutine(checkLobbyState());
		}

		/// <summary>스팀 로비의 상태를 주기적으로 갱신합니다.</summary>
		public IEnumerator checkLobbyState()
		{
			while (true)
			{
				if (mSessionHandler == null)
				{
					continue;
				}

				if (!mSessionHandler.TryGetCurrentLobby(out var currentLobby))
				{
					continue;
				}

				Text_LobbyName.Text = currentLobby.GetLobbyName();

				yield return new WaitForSeconds(2.0f);
			}
		}

		private void onSessionChanged(IngameSessionHandler handler)
		{
			Nav_LobbySessions.Setup(handler);

			refreshReadyButton();
			refreshSelectButton(handler);
		}

		private void refreshReadyButton()
		{
			if (mSessionHandler.IsServerSide)
			{
				Text_ReadyOrStart.Text = "시작하기";

				if (mSessionHandler.Server_AreAllPlayerReadyToPlay())
				{
					Btn_ReadyOrStart.SetInteractable(true);
					Btn_ReadyOrStart.BindAction(() => mSessionHandler.Server_OnStartGame());
				}
				else
				{
					Btn_ReadyOrStart.SetInteractable(false);
				}
			}
			else
			{
				if (mSessionHandler.TryGetMySessionInfo(out var s))
				{
					Btn_ReadyOrStart.SetInteractable(s.Character.GetEnum() != CharacterType.None);

					bool isCurrentlyReady = mSessionHandler.IsClientReady();
					Btn_ReadyOrStart.BindAction(() => mSessionHandler.OnRequestReady(!isCurrentlyReady));
					Text_ReadyOrStart.Text = isCurrentlyReady ? "준비해제" : "준비하기";
				}
			}
		}

		private void refreshSelectButton(IngameSessionHandler handler)
		{
			foreach (var b in mCharacterSelectorTable.Values)
			{
				b.SetInteractable(true);
			}

			foreach (var s in handler.IngameSessions)
			{
				if (mCharacterSelectorTable.TryGetValue(s.Character, out var button))
				{
					button.SetInteractable(false);
				}
			}
		}

	}
}
