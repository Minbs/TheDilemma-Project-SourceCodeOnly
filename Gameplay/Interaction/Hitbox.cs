using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Gameplay
{

	public abstract class Hitbox : MonoBehaviour
	{
		[Title("데미지 비율")]
		[Range(0.0F, 2.0F)] public float DamageMultiplier = 1;
		[field: SerializeField] public EntityBase Entity { get; protected set; }

#if UNITY_EDITOR
		[Button]
		public void SetupEntity()
		{
			Entity = GetComponentInParent<EntityBase>();
			gameObject.layer = GlobalLayer.LAYER_ENTITY_HITBOX;
		}

		public virtual void OnValidate()
		{
			DamageMultiplier = (int)(DamageMultiplier * 8) / 8.0F;
		}
#endif
	}

	public abstract class Hitbox<T> : Hitbox where T : Collider2D
	{
		[field: SerializeField] public T HitboxCollider { get; private set; }

		public Color GetHitboxColor()
		{
			Color returnColor;

			if (DamageMultiplier < 1)
			{
				float t = DamageMultiplier;
				returnColor = Color.blue * (1 - t) + Color.yellow * t;
			}
			else
			{
				float t = DamageMultiplier - 1;
				float a = (1 - t) < 0 ? 0 : 1 - t;
				returnColor = Color.yellow * a + Color.red * t;
			}

			returnColor.a = 0.3f;
			return returnColor;
		}

#if UNITY_EDITOR
		public override void OnValidate()
		{
			base.OnValidate();
			HitboxCollider = GetComponent<T>();
		}
#endif
	}
}
