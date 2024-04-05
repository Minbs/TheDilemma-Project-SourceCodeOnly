using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay
{
	public class HitboxBox : Hitbox<BoxCollider2D>
	{
#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			var size = HitboxCollider.size;
			var position = HitboxCollider.offset + transform.position.ToVector2() - size * 0.5f;
			Handles.DrawSolidRectangleWithOutline(new Rect(position, size), GetHitboxColor(), GetHitboxColor() * 2);
		}
#endif
	}
}
