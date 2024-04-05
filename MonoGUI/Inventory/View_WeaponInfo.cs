using System;
using UnityEngine;
using Utils.ViewModel;
namespace MonoGUI
{
	public class View_WeaponInfo : ToolTip_ScreenView
	{
		[SerializeField]
		private ImageViewModel Img_Item = new(nameof(Img_Item));
		[SerializeField]
		private TextMeshProTextViewModel Text_ItemName = new(nameof(Text_ItemName));
		private LocalizationText mLocalItemName;

		[SerializeField]
		private TextMeshProTextViewModel Text_ItemInfo = new(nameof(Text_ItemInfo));
		private LocalizationText mLocalItemInfo;

		[SerializeField]
		private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));

		public override void OnInitialized()
		{
			Img_Item.Initialize(this);
			Text_ItemName.Initialize(this);
			Text_ItemInfo.Initialize(this);
			Btn_Exit.Initialize(this);
			Btn_Exit.BindAction(OnClose);
		}

		public void Initialized(Sprite item, string nameEntryKey, string infoEntryKey)
		{
			Img_Item.Sprite = item;
			mLocalItemName = new(Text_ItemName, nameEntryKey);
			mLocalItemInfo = new(Text_ItemInfo, infoEntryKey);
		}

		public override void OnClose()
		{
			mOnClose?.Invoke();
		}
	}

}