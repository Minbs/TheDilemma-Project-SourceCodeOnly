using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;
using Sirenix.OdinInspector;
using Utils;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace MonoGUI
{
	public class View_Escape : MonoGUI_View
	{
		[SerializeField] private GameObjectViewModel Content_MenuButtonPrefab = new(nameof(Content_MenuButtonPrefab));

		[Title("QuickButton Option")]
		[SerializeField] private RectTransform QuickButtonsRectTransform;

		[Title("BaseButton Option")]
		[SerializeField] private RectTransform BaseButtonsRectTransfrom;

		private List<GameObject> mButtons = new List<GameObject>();

		public override void OnInitialized()
		{
			Content_MenuButtonPrefab.Initialize(this);
		}

		public void Initialize(IList<EscapeButtonInfo> buttonInfos)
		{
			foreach (var obj in mButtons)
			{
				Destroy(obj);
			}

			mButtons.Clear();

			foreach (var info in buttonInfos)
			{
				addMenuButton(info.ButtonName, info.OnClick, info.IsQuickMenu);
			}

			void addMenuButton(string buttonName, Action onClickCallback, bool isQuickMenu)
			{
				RectTransform target = isQuickMenu ? QuickButtonsRectTransform : BaseButtonsRectTransfrom;

				var go = Instantiate(Content_MenuButtonPrefab.GameObject, target);

				// Initialize
				go.GetComponent<EscapeMenuButton>().Initialize(buttonName, onClickCallback);
				go.SetActive(true);

				mButtons.Add(go);
			}
		}
	}
}
