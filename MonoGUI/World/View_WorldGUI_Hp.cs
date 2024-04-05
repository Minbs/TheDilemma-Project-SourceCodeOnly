using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils.ViewModel;
using System;
namespace MonoGUI
{
	public class View_WorldGUI_Hp : ToolTip_WorldView
	{
		[SerializeField]
		private SliderViewModel Slider_HP = new(nameof(Slider_HP));


		public void Initialize(RectTransform drawRect, Transform target)
		{
			InitializeToolTip(drawRect, true);
			mTarget = target;
		}

		public override void OnInitialized()
		{
			Slider_HP.Initialize(this);
		}

		public void DrawHP(int current, int max)
		{
			var p = current / max;
			Slider_HP.Value = p;
		}

		public void Draw(float percent)
		{
			Slider_HP.Value = percent;
		}

	}
}
