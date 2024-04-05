using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	/// <summary>생성 즉시 판단하고 이펙트를 생성합니다.</summary>
	public class Hitscan_InstantSingle : HitscanBase
	{
		public override void OnCreated()
		{
			var start = HitscanInfo.Start;
			var direction = HitscanInfo.Direction;
			var distance = HitscanInfo.WeaponInfo.MaxDistance;

			var hits = Physics2D.RaycastAll
			(
				start, 
				direction, 
				distance, 
				GlobalLayer.LAYER_RAYCAST_HITBOX
			);

			int penetrateCount = HitscanInfo.WeaponInfo.PenetrateCount;
			List<RaycastHit2DInfo> raycastHitInfos = new();

			Vector2 initialEnd = start + direction.Value * distance;
			Vector2 end = initialEnd;

			HashSet<NetObjectID> hitset = new();

			foreach (var hit in hits)
			{
				var entityHitbox = hit.collider.GetComponent<Hitbox>();
				if (entityHitbox == null)
				{
					continue;
				}

				if (entityHitbox.Entity.ID == this.HitscanInfo.Attacker.ObjectID)
				{
					continue;
				}

				if (!hitset.Add(entityHitbox.Entity.ID))
				{
					continue;
				}

				raycastHitInfos.Add(new RaycastHit2DInfo(entityHitbox, hit));
				end = hit.point;

				penetrateCount--;
				if (penetrateCount <= 0)
				{
					break;
				}
			}

			if (penetrateCount > 0)
			{
				end = initialEnd;
			}

			HitscanInfo = new HitscanInfo(HitscanInfo, raycastHitInfos);
			mHitscanHandler.Client_RequestPerform(HitscanID, HitscanInfo);
			mHitscanHandler.EffectHandler.CreateSyncHitscanEffect(HitscanInfo.WeaponInfo.EffectType, start, end);

			OnRelease();
		}

		public override void OnRelease()
		{
			mHitscanHandler.Release(this);
		}

		public override void OnPerformed(HitscanInfo hitscanInfo)
		{
			OnRelease();
		}
	}
}
