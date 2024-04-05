using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Utils.ViewModel;

namespace MonoGUI
{
	public class DialogButton : MonoBehaviour
	{
		[SerializeField] private TextMeshProTextViewModel Text_Dialog = new(nameof(Text_Dialog));
		[SerializeField] private ButtonViewModel Btn_Dialog = new(nameof(Btn_Dialog));

		private DialogResult mDialogResult;
		public event Action OnButtonClick;
		public event Action<DialogResult> OnResultClick;

		public void Initialize((DialogResult Result, bool IsInteractable) dialogResult)
		{
			mDialogResult = dialogResult.Result;

			Text_Dialog.Initialize(this);
			Text_Dialog.Text = mDialogResult.GetText();

			Btn_Dialog.Initialize(this);
			Btn_Dialog.BindAction(() =>
			{
				OnResultClick?.Invoke(mDialogResult);
				OnButtonClick?.Invoke();
			});
			Btn_Dialog.SetInteractable(dialogResult.IsInteractable);
		}
	}
}
