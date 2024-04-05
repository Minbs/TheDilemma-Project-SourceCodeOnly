using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils.ViewModel;
namespace MonoGUI
{
	public class View_TestItemInfo : MonoGUI_View
	{
		[SerializeField]
		private ButtonViewModel Btn_Item = new(nameof(Btn_Item));
		[SerializeField]
		private Navigation_ScreenGUI mNavigation_ItemToolTip;
		[SerializeField]
		private TestItemInfo mTestItemInfo;

		public override void OnInitialized()
		{
			Btn_Item.Initialize(this);
			Btn_Item.BindAction(onOpenItemInfo);
		}

		private void onOpenItemInfo()
		{
			var mouseScreenPos = Input.mousePosition;
			Debug.Log("MousePOS:" + mouseScreenPos);
			mNavigation_ItemToolTip.OpenScreenGUI<View_WeaponInfo>(mouseScreenPos, false, out var view);
			view.Initialized(mTestItemInfo.Item, mTestItemInfo.NameEntrykey, mTestItemInfo.InfoEntryKey);
		}

	}
}
