using KaNet.Synchronizers;
using Sirenix.OdinInspector;
using UnityEngine;
using Utile;

namespace Gameplay
{
	public abstract class HitscanBase : MonoBehaviour
	{
		[field: SerializeField] public HitscanType Type { get; private set; }

		protected HitscanHandler mHitscanHandler;
		public NetObjectID HitscanID { get; private set; }
		public HitscanInfo HitscanInfo { get; protected set; }
		/// <summary>프록시 여부입니다. Proxy인 경우 피격대상에게 실질적인 영향을 미치지 않습니다.</summary>

#if UNITY_EDITOR
		[Button]
		public void SetupPrefabNameByType()
		{
			gameObject.name = Type.ToString();
		}
#endif

		public virtual void Initialize
		(
			HitscanHandler handler,
			HitscanInfo hitscanInfo,
			NetObjectID hitscanID
		)
		{
			mHitscanHandler = handler;
			HitscanID = hitscanID;
			HitscanInfo = hitscanInfo;

			OnCreated();
		}

		/// <summary>히트스캔이 생성되었을 때 이벤트 입니다.</summary>
		public abstract void OnCreated();

		public abstract void OnRelease();

		public abstract void OnPerformed(HitscanInfo hitscanInfo);
	}
}
