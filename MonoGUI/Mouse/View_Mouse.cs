using UnityEngine;
using Utils.ViewModel;

namespace MonoGUI
{
	public abstract class View_Mouse : MonoGUI_View
	{
		protected readonly Vector2 mRecoilOffsetMin = new Vector2(1.0f, 1.0f);
		protected readonly Vector2 mRecoilOffsetMax = new Vector2(-1.0f, -1.0f);

		private void Update()
		{
			MoveToRealMousePoint();
		}

		public abstract void OnClickAction();

		/// <summary>
		/// 총기 반동을 GUI로 표현합니다.
		/// 최종 값은 0.0f ~ 1.0f 사의 값을 가지게됩니다.
		/// </summary>
		/// <param name="recoilPercent">추가할 반동 퍼센트</param>
		public abstract void ApplyRecoil(float recoilPercent);

		/// <summary> 총기 반동 GUI를 리셋합니다. </summary>
		public abstract void ResetRecoil();

		public abstract void OnReload();

		/// <summary> 반동 GUI에서 앵커 늘어남으로 연출 할 때 사용</summary>
		/// <param name="offset"></param>
		/// <param name="aim"></param>
		public void SetRecoilOffset(float offset, RectTransfromViewModel aim)
		{
			Vector2 recoilMin = mRecoilOffsetMin * offset;
			Vector2 recoilMax = mRecoilOffsetMax * offset;
			aim.SetOffsetMax(recoilMax);
			aim.SetOffsetMin(recoilMin);
		}

		public void MoveToRealMousePoint()
		{
			ViewRectTransfrom.position = Input.mousePosition;
		}

		public Vector3 GetScreenPosition()
		{
			return ViewRectTransfrom.position;
		}

	}
}
