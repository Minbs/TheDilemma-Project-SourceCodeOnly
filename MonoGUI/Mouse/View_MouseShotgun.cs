using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.ViewModel;
using UnityEngine;
namespace MonoGUI
{
	public class View_MouseShotgun : View_Mouse
	{
		[SerializeField]
		private RectTransfromViewModel Rect_Aim = new(nameof(Rect_Aim));

		[field: SerializeField]
		public float MinRecoilAimOffset { private set; get; }
		[field: SerializeField]
		public float MaxRecoilAimOffset { private set; get; }


		private float mCurrentRecoilPercent;

		public void Initiailzed()
		{
			SetRecoilOffset(MinRecoilAimOffset, Rect_Aim);
		}

		public override void ApplyRecoil(float recoil)
		{
			mCurrentRecoilPercent += recoil;
			mCurrentRecoilPercent = Mathf.Clamp(mCurrentRecoilPercent, 0.0f, 1.0f);

			var currentOffSet = Mathf.Lerp(MinRecoilAimOffset, MaxRecoilAimOffset, mCurrentRecoilPercent);

			SetRecoilOffset(currentOffSet, Rect_Aim);
		}

		public override void OnClickAction()
		{
		}

		public override void OnInitialized()
		{
			Rect_Aim.Initialize(this);
		}

		public override void OnReload()
		{

		}

		public override void ResetRecoil()
		{
			mCurrentRecoilPercent = 0.0f;
			var currentOffSet = Mathf.Lerp(MinRecoilAimOffset, MaxRecoilAimOffset, mCurrentRecoilPercent);
			SetRecoilOffset(currentOffSet, Rect_Aim);
		}
	}
}
