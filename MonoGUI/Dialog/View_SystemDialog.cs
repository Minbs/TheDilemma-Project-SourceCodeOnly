using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using UnityEngine;
using Utils.ViewModel;

namespace MonoGUI
{
	public enum DialogResult
	{
		None = 0,
		OK, // 확인
		Yes, // 네
		No, // 아니요
		Apply, // 적용
		Cancel, // 취소
	}

	public static class DialogResultExtension
	{
		public static string GetText(this DialogResult dialogResult)
		{
			return dialogResult.ToString();
		}
	}

	public class View_SystemDialog : MonoGUI_View
	{
		public NetOperationType OperationType { get; private set; }

		[SerializeField]
		private TextMeshProTextViewModel Text_TitleName = new(nameof(Text_TitleName));

		[SerializeField]
		private TextMeshProTextViewModel Text_Context = new(nameof(Text_Context));

		[SerializeField]
		private TransformViewModel Transform_Buttons = new(nameof(Transform_Buttons));

		[SerializeField]
		private GameObjectViewModel Content_DialogButtonPrefab = new(nameof(Content_DialogButtonPrefab));

		private List<GameObject> mDialogButtonList = new();

		public event Action<DialogResult> OnResultCallback;
		public event Action<View_SystemDialog> OnButtonClicked;

		public override void OnInitialized()
		{
			Text_TitleName.Initialize(this);
			Text_Context.Initialize(this);
			Transform_Buttons.Initialize(this);
			Content_DialogButtonPrefab.Initialize(this);
		}

		public View_SystemDialog ShowDialog
		(
			NetOperationType operationType,
			string title,
			string context,
			params (DialogResult Result, bool IsInteractable)[] dialogResults
		)
		{
			OperationType = operationType;

			// Initialize
			OnResultCallback = null;
			foreach (var go in mDialogButtonList)
			{
				Destroy(go);
			}
			mDialogButtonList.Clear();

			// Set Text
			Text_TitleName.Text = title;
			Text_Context.Text = context;

			// Create Buttons
			foreach (var result in dialogResults)
			{
				if (result.Result == DialogResult.None)
				{
					continue;
				}

				var button = Instantiate(Content_DialogButtonPrefab.GameObject, Transform_Buttons.Transform)
					.GetComponent<DialogButton>();

				button.Initialize(result);
				button.OnButtonClick += () => { OnButtonClicked?.Invoke(this); };
				button.OnResultClick += (result) => { OnResultCallback?.Invoke(result); };
				button.gameObject.SetActive(true);
				mDialogButtonList.Add(button.gameObject);
			}

			// Show Dialog
			Show();
			return this;
		}
	}

}
