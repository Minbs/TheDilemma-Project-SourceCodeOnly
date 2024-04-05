using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Gameplay
{

	public class LocalHitscan_Box : LocalHitscanBase
	{
		[field : SerializeField] public GameObject EffectObject { get; private set; }
		[field : SerializeField] public float HitscanTime { get; private set;} 
		[field : SerializeField] public Vector2 HitboxSize { get; private set; }

		public override void Perform(bool isServerSide)
		{
			EffectObject.SetActive(true);

			if (isServerSide)
			{
				StartCoroutine(performAsync(HitscanTime));
			}
		}

		private IEnumerator performAsync(float performTimeAt)
		{
			yield return new WaitForSeconds(performTimeAt);

			var hits = Physics2D.BoxCastAll
			(
				transform.position,
				HitboxSize,
				0,
				Vector2.right,
				0,
				GlobalLayer.LAYER_RAYCAST_HITBOX
			);

			Dictionary<EntityBase, Hitbox> hitset = new();

			foreach (var hit in hits)
			{
				var entityHitbox = hit.collider.GetComponent<Hitbox>();
				if (entityHitbox == null)
				{
					continue;
				}

				if (entityHitbox.Entity.ID == this.OwnerEntity.ObjectID)
				{
					continue;
				}

				if (!AllowFriendlyFire &&
					entityHitbox.Entity.Faction.IsAlliance(OwnerEntity.Faction))
				{
					continue;
				}

				if (!hitset.TryAdd(entityHitbox.Entity, entityHitbox))
				{
					continue;
				}
			}

			foreach (var e in hitset.Keys)
			{
				var hitbox = hitset[e];
				e.OnHitBy(OwnerEntity.Faction, hitbox.DamageMultiplier, Damage);
			}
		}

#if UNITY_EDITOR
		[Title("Gizmo Setting")]
		public Color GizmoColor = new Color(1, 0, 1, 0.5f);

		public void OnDrawGizmos()
		{
			Handles.matrix = transform.localToWorldMatrix;
			Handles.DrawSolidRectangleWithOutline
			(
				new Rect(HitboxSize * -0.5f, HitboxSize),
			GizmoColor,
			GizmoColor * 0.5f
			);
		}
#endif
	}
}
