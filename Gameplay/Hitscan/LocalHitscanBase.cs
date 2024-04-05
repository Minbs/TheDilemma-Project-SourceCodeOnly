using UnityEngine;

namespace Gameplay
{
	public abstract class LocalHitscanBase : MonoBehaviour
	{
		[field: SerializeField] public EntityBase OwnerEntity { get; private set; }
		[field: SerializeField] public int Damage { get; private set; }
		[field: SerializeField] public bool AllowFriendlyFire { get; private set; }

		public abstract void Perform(bool isServerSide);
	}
}
