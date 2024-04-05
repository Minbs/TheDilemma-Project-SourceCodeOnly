using KaNet.Session;
using MonoGUI;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_Terminal_ServerBrowser : MonoGUI_View
	{
		[SerializeField] private ScrollRectViewModel Scroll_LobbyList = new(nameof(Scroll_LobbyList));
		[SerializeField] private ButtonViewModel Btn_RefreshLobbyList = new(nameof(Btn_RefreshLobbyList));
		[SerializeField] private GameObjectViewModel Content_LobbyLabel = new(nameof(Content_LobbyLabel));

		private RectTransform mElementRect;
		private float mElementHeight;
		private float mElementHalfHeight;

		private List<Content_LobbyLabel> mLobbyLabelList = new List<Content_LobbyLabel>();
		private Action mOnClickRefreshLobbyList;

		[SerializeField] private MonoGUI_ListBox<Content_LobbyLabel> mFriendListBox = new MonoGUI_ListBox<Content_LobbyLabel>();

		public override void OnInitialized()
		{
			Scroll_LobbyList.Initialize(this);
			Btn_RefreshLobbyList.Initialize(this);
			Btn_RefreshLobbyList.BindAction(() => mOnClickRefreshLobbyList?.Invoke());
			Content_LobbyLabel.Initialize(this);

			#region ElementObjet Setting

			if (!Content_LobbyLabel.GameObject.TryGetComponent<RectTransform>(out mElementRect))
			{
				Ulog.LogError(UlogType.UI, $"{Content_LobbyLabel.GameObject.name} is don't have RectTransform");
				return;
			}
			mElementHeight = mElementRect.sizeDelta.y;
			mElementHalfHeight = mElementHeight * 0.5f;

			#endregion
		}

		public void Initialize(Action onClickRefreshLobbyList)
		{
			mOnClickRefreshLobbyList = onClickRefreshLobbyList;

			mFriendListBox.Initialize(Scroll_LobbyList, ListBoxAddDiraction.Down);
		}

		private GameObject contentInstance(GameObject prefab, Transform parent)
		{
			return Instantiate(prefab, parent);
		}

		private void contentDestroy(GameObject content)
		{
			Destroy(content);
		}

		public void RefreshLobbyList(IEnumerable<Lobby> lobbyList, Action<EndPointInfo> onClick_JoinLobbyCallback)
		{
			mFriendListBox.Clear();
			foreach (var lobby in lobbyList)
			{
				var lobbyLabelContent = Instantiate(Content_LobbyLabel.GameObject).GetComponent<Content_LobbyLabel>();
				lobbyLabelContent.SetContextAsync(lobby, onClick_JoinLobbyCallback);

				mFriendListBox.Add(lobbyLabelContent);
			}
		}
	}
}

