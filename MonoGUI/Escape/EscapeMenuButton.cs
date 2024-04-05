using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.ViewModel;

namespace MonoGUI
{
	public class EscapeMenuButton : MonoBehaviour
	{
		[SerializeField] private TextMeshProTextViewModel Text_EscapeMenu = new(nameof(Text_EscapeMenu));
		[SerializeField] private ButtonViewModel Btn_EscapeMenu = new(nameof(Btn_EscapeMenu));

		private Action mOnClick;

		public void Initialize(string escapeMenuText, Action onClickCallback)
		{
			mOnClick = onClickCallback;

			Text_EscapeMenu.Initialize(this);
			Text_EscapeMenu.Text = escapeMenuText;

			Btn_EscapeMenu.Initialize(this);
			Btn_EscapeMenu.BindAction(() =>
			{
				mOnClick?.Invoke();
			});
		}
	}
}
