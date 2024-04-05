using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using UnityEngine;

namespace MonoGUI
{
	public class Navigation_SystemDialog : MonoGUI_Navigation
	{
		public void Initialize()
		{
			GlobalServiceLocator
				.NetworkManageService
				.GetServiceOrNull()
				.OnNetworkCallback += onCallback;
		}

		public View_SystemDialog ShowSystemDialog
		(
			NetOperationType operationType,
			string title,
			string context,
			params (DialogResult Result, bool IsInteractable)[] dialogResults
		)
		{
			var dialog = Push<View_SystemDialog>();
			dialog.ShowDialog(operationType, title, context, dialogResults);
			dialog.OnButtonClicked += onDialogButtonClicked;

			return dialog;
		}

		/// <summary>다이얼로그 응답 버튼이 눌리면 해당 창을 닫습니다.</summary>
		private void onDialogButtonClicked(View_SystemDialog dialog)
		{
			PopByObject(dialog.gameObject);
		}

		private void onCallback(NetCallback netCallback)
		{
			PopMatch((e) =>
			{
				var dialog = e.View as View_SystemDialog;
				if (dialog == null)
				{
					return false;
				}

				return dialog.OperationType == netCallback.Operation;
			});

			if (netCallback.Result != NetOperationResult.Success)
			{
				this.ShowSystemDialog
				(
					netCallback.Operation,
					netCallback.Operation.GetTitle(),
					netCallback.Result.GetMessage(),
					(DialogResult.OK, true)
				);
			}
		}

		public void OnCallback(NetOperationType operationType)
		{
			PopMatch((e) =>
			{
				var dialog = e.View as View_SystemDialog;
				if (dialog == null)
				{
					return false;
				}

				return dialog.OperationType == operationType;
			});
		}
	}
}
