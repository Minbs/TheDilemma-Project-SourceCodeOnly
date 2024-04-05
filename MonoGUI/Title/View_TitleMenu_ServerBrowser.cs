using KaNet.Session;
using KaNet.SteamworksAPI;
using KaNet.Synchronizers;
using System;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_TitleMenu_ServerBrowser : MonoGUI_View
	{
		[SerializeField] private ScrollRectViewModel Scroll_LobbyList = new(nameof(Scroll_LobbyList));
		[SerializeField] private GameObjectViewModel Content_LobbyLabel = new(nameof(Content_LobbyLabel));
		[SerializeField] private ButtonViewModel Btn_Connection = new(nameof(Btn_Connection));

		[SerializeField] private TextMeshProTextViewModel Text_LobbyInfo_Name = new(nameof(Text_LobbyInfo_Name));
		[SerializeField] private TextMeshProTextViewModel Text_LobbyInfo_Description = new(nameof(Text_LobbyInfo_Description));
		[SerializeField] private TextMeshProTextViewModel Text_LobbyInfo_Personnel = new(nameof(Text_LobbyInfo_Personnel));

		[SerializeField] private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));

		private RectTransform mElementRect;
		private float mElementHeight;
		private float mElementHalfHeight;

		private Action mOnClickRefreshLobbyList;

		private MonoGUI_ListBox<TitleMenu_Content_LobbyLabel> mLobbyListBox = new MonoGUI_ListBox<TitleMenu_Content_LobbyLabel>();

		private NetworkManageService mNetworkManageService;

		private EndPointInfo mEndPointInfo;
		private bool mIsLobbySelected;


		public override void OnInitialized()
		{
			mNetworkManageService = GlobalServiceLocator.NetworkManageService.GetServiceOrNull();
			Scroll_LobbyList.Initialize(this);
			Content_LobbyLabel.Initialize(this);

			Text_LobbyInfo_Name.Initialize(this);
			Text_LobbyInfo_Description.Initialize(this);
			Text_LobbyInfo_Personnel.Initialize(this);

			Btn_Exit.Initialize(this);

			Btn_Connection.Initialize(this);
			Btn_Connection.BindAction(() =>
			{
				if (!mIsLobbySelected)
				{
					return;
				}

				tryConnect(mEndPointInfo, new NetConnectRequestInfo
						(
							ProcessHandler.Instance.ID,
							"" // Password
						));
			});

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

		public void Initialized(Action exitAction)
		{
			mIsLobbySelected = false;

			mLobbyListBox.Initialize(Scroll_LobbyList, ListBoxAddDiraction.Down);
			refreshLobbyList((endPoint) =>
			{
				mIsLobbySelected = true;
				mEndPointInfo = endPoint;
				Text_LobbyInfo_Name.Text = endPoint.TargetLobby.GetLobbyName();
				Text_LobbyInfo_Description.Text = endPoint.TargetLobby.GetLobbyDescription();
				Text_LobbyInfo_Personnel.Text = endPoint.TargetLobby.MemberCount.ToString();
			});

			Btn_Exit.BindAction(() =>
			{
				exitAction?.Invoke();
			});
		}

		private GameObject contentInstance(GameObject prefab, Transform parent)
		{
			return Instantiate(prefab, parent);
		}

		private void contentDestroy(GameObject content)
		{
			Destroy(content);
		}

		private void refreshLobbyList(Action<EndPointInfo> onClick_LobbyCallback)
		{
			mNetworkManageService.RequestLobbyList((lobbys) =>
			{
				mLobbyListBox.Clear();
				foreach (var lobby in lobbys)
				{
					var version = lobby.GetGameVersion();
					if (!string.IsNullOrEmpty(version))
					{
						var lobbyLabelContent = Instantiate(Content_LobbyLabel.GameObject).GetComponent<TitleMenu_Content_LobbyLabel>();
						lobbyLabelContent.SetContextAsync(lobby, onClick_LobbyCallback);
						mLobbyListBox.Add(lobbyLabelContent);
					}

				}
			});
		}

		private void tryConnect(EndPointInfo endPointInfo, NetConnectRequestInfo requestInfo)
		{
			var globalGui = GlobalServiceLocator.GlobalGuiService.GetServiceOrNull();

			globalGui.ShowSystemDialog
			(
				NetOperationType.TryConnect,
				NetOperationType.TryConnect.GetTitle(),
				"¡¢º”¡ﬂ...",
				(DialogResult.None, false)
			);

			mNetworkManageService.TryConnectTo(endPointInfo, requestInfo);
		}

	}
}

