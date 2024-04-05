using UnityEngine;
using Utils;
using Utils.ViewModel;

using Sirenix.OdinInspector;
using System;

namespace MonoGUI
{
	public class View_TitleMenu_SinglePlay : MonoGUI_View
	{
		[SerializeField] ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));

		private Action mExitAction;

		public override void OnInitialized()
		{
			Btn_Exit.Initialize(this);
			Btn_Exit.BindAction(() =>
			{
				mExitAction?.Invoke();
			});
		}

		public void Initialized(Action exitAction)
		{
			mExitAction = exitAction;
		}

	}
}