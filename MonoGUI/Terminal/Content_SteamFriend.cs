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

namespace MonoGUI
{
	public class Content_SteamFriend : MonoGUI_View
	{
		[SerializeField] private RawImageViewModel Img_UserAvatar = new(nameof(Img_UserAvatar));
		[SerializeField] private TextMeshProTextViewModel Text_FriendName = new(nameof(Text_FriendName));
		[SerializeField] private ButtonViewModel Btn_Invite = new(nameof(Btn_Invite));

		private Friend? mFriendInfo;
		private Action<Friend> mOnClick_InviteFriendCallback;

		public bool IsSetup => mFriendInfo != null;

		public override void OnInitialized()
		{
			Img_UserAvatar.Initialize(this);
			Text_FriendName.Initialize(this);

			Btn_Invite.Initialize(this);
			Btn_Invite.BindAction(() =>
			{
				if (!IsSetup)
				{
					Ulog.Log(this, $"It hasn't set up friend information.");
				}

				mOnClick_InviteFriendCallback?.Invoke(mFriendInfo.Value);
			});
		}

		public async Task SetContextAsync(Friend friend, Action<Friend> onClickCallback)
		{
			mFriendInfo = friend;
			mOnClick_InviteFriendCallback = onClickCallback;

			// Setup Name
			Text_FriendName.Text = mFriendInfo.Value.Name;

			// Setup avatar
			var avatar = await SteamUtilExtension.GetTextureFromSteamIDAsync(mFriendInfo.Value.Id);
			Img_UserAvatar.Texture = avatar;
		}
	}
}
