using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.ViewModel;
namespace MonoGUI
{
	public class View_MouseNormal : View_Mouse
	{
		[SerializeField]
		private ImageViewModel Img_MouseCursor = new(nameof(Img_MouseCursor));

		public override void ApplyRecoil(float recoil)
		{
		}

		public override void OnClickAction()
		{
			Img_MouseCursor.Initialize(this);
		}

		public override void OnInitialized()
		{
			
		}

		public override void OnReload()
		{
		}

		public override void ResetRecoil()
		{
		}
	}
}
