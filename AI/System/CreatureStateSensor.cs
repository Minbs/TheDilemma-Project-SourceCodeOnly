using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;
using UnityEngine;

namespace NetworkAI
{
	public static class CreatureStateSensor
	{
		/// <summary> 플레이어가 시야 원형 범위안에 있는지 확인합니다.</summary>
		/// <param name="creature">찾는 대상</param>
		/// <param name="factionMatch">찾는 팀 규칙</param>
		/// <param name="radius">반경</param>
		/// <param name="target">찾은 대상</param>
		/// <returns>대상을 찾았으면 true를 반환합니다.</returns>
		public static bool TryGetTargetInCircleRange
		(
			Entity_Creature creature, 
			FactionMatchType factionMatch, 
			float radius, 
			out EntityBase target
		)
		{
			Vector3 position = creature.transform.position;

			var collideList = Physics2D.OverlapCircleAll
			(
				position, 
				radius, 
				GlobalLayer.LAYER_RAYCAST_ENTITY_AREA
			);

			foreach (Collider2D other in collideList)
			{
				var otherEntity = other.GetComponent<EntityBase>();

				var entityFaction = creature.Faction;
				var otherFaction = otherEntity.Faction;

				if (entityFaction.IsMatch(otherFaction, factionMatch))
				{
					target = otherEntity;
					return true;
				}
			}

			target = null;
			return false;
		}
	}
}
