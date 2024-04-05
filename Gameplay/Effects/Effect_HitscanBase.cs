using Sirenix.OdinInspector;
using UnityEngine;
using Utils.Service;

namespace Gameplay
{
	/// <summary>레이저 처럼 시작과 끝점을 한 번에 검출하는 Hitscan입니다.</summary>
	public abstract class Effect_HitscanBase : MonoBehaviour
	{
		[field : SerializeField] public EffectType Type { get; private set; }

		public const int DIVIDE_OFFSET = 8;
		public const float ONE_UNIT_DIVISION = 1.0F / DIVIDE_OFFSET;

		private MonoObjectPoolService mObjectPool;

		public void Awake()
		{
			mObjectPool = new MonoObjectPoolService(transform);
		}

#if UNITY_EDITOR
		[Button]
		public void SetupPrefabNameByType()
		{
			gameObject.name = Type.ToString();
		}
#endif

		public virtual void InitializeHitscan(Vector3 start, Vector3 direction, float distance)
		{
			var end = start + direction.normalized * distance;
			InitializeHitscan(start, end);
		}

		public abstract void InitializeHitscan(Vector3 start, Vector3 end);

		#region

		protected GameObject createObject(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return mObjectPool.CreateObject(prefab, position, rotation);
		}

		protected void releaseObject(GameObject instance)
		{
			mObjectPool.Release(instance);
		}

		#endregion
	}
}
