using UnityEngine;
using Utils;
using Utils.ViewModel;

using Sirenix.OdinInspector;
using System;

namespace MonoGUI
{
	public class View_Terminal_SinglePlay : MonoGUI_View
	{
		[SerializeField] private ImageViewModel Img_Map = new(nameof(Img_Map));
		[SerializeField] private ButtonViewModel Btn_Play = new(nameof(Btn_Play));

		private Action mOnSinglePlay;

		public override void OnInitialized()
		{
			Img_Map.Initialize(this);
			Btn_Play.Initialize(this);
			Btn_Play.BindAction(onSinglePlay);
		}

		public void Initialize(Action onSinglePlay)
		{
			mOnSinglePlay = onSinglePlay;
		}

		private void onSinglePlay()
		{
			mOnSinglePlay?.Invoke();
		}
	}
}