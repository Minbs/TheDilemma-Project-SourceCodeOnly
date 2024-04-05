using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
#endif

namespace Gameplay
{
	public abstract class LocatorBase : MonoBehaviour
	{
		[Title("Locator Setting")]
		[field: SerializeField]public float DetectTick { get; private set; } = 0.125f;
		[field: SerializeField]public bool IsServerSide { get; private set; } = true;
		[field: SerializeField]public bool IsSingleUse { get; private set; } = true;

		public GameplayManager GameplayManager { get; private set; }

		public void Initialize(GameplayManager gameplayManager)
		{
			GameplayManager = gameplayManager;
		}

		public abstract void StartLocator(bool isCurrentlyServerSide);
		public abstract void StopLocator();
	}
}
