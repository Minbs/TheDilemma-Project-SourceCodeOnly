using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Gameplay
{
	public abstract class EntityTriggerEvent : MonoBehaviour
	{
		public MapHandler Handler { get; protected set; }
		public GameplayManager GameplayManager => Handler.GameplayManager;

		public void InitializeBy(MapHandler handler)
		{
			Handler = handler;
		}

		/// <summary>Entity가 감지되면 호출됩니다. 설정에 따라서 지속적으로 호출됩니다.</summary>
		/// <param name="entityBases"></param>
		public abstract void OnDetected(IList<EntityBase> entityBases);

		/// <summary>감지되던 Entity가 더 이상 감지되지 않으면 한 번 호출됩니다.</summary>
		public abstract void OnUndetected();
	}
}
