using KaNet.Session;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_Terminal_InviteFriends : MonoGUI_View
	{
		[SerializeField] private ScrollRectViewModel Scroll_FriendList = new(nameof(Scroll_FriendList));
		[SerializeField] private GameObjectViewModel Content_SteamFriend = new(nameof(Content_SteamFriend));

		private RectTransform mElementRect;

		private List<Content_SteamFriend> mFriendList = new List<Content_SteamFriend>();
		private Action<NetSessionInfo> mOnClick_InviteFriendCallback;

		[SerializeField] private MonoGUI_ListBox<Content_SteamFriend> mFriendListBox = new();

		public override void OnInitialized()
		{
			Scroll_FriendList.Initialize(this);
			mFriendListBox.Initialize(Scroll_FriendList, ListBoxAddDiraction.Down);
		}

		private GameObject contentInstance(GameObject prefab, Transform parent)
		{
			return Instantiate(prefab, parent);
		}

		private void contentDestroy(GameObject content)
		{
			Destroy(content);
		}

		public void Clear()
		{
			for (int i = mFriendList.Count - 1; i >= 0; i--)
			{
				Destroy(mFriendList[i].gameObject);
			}

			mFriendList.Clear();
		}

		public void Initialize
		(
			IEnumerable<Friend> friendList, 
			Action<NetSessionInfo> onClick_InviteFriendCallback
		)
		{
			mFriendListBox.Clear();

			mOnClick_InviteFriendCallback = onClick_InviteFriendCallback;

			foreach (var friend in friendList)
			{
				var newFriendInviteView =
					Instantiate(Content_SteamFriend.GameObject)
					.GetComponent<Content_SteamFriend>();

				// Set friend information
				var setup = newFriendInviteView.SetContextAsync(friend, onClick_InviteFriend);
				mFriendListBox.Add(newFriendInviteView);
			}
		}

		private void onClick_InviteFriend(Friend friend)
		{
			mOnClick_InviteFriendCallback?.Invoke(new NetSessionInfo(friend));
		}
	}
}
