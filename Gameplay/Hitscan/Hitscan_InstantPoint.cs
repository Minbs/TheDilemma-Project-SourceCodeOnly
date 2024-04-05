using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	/// <summary>생성 즉시 판단하고 이펙트를 생성합니다.</summary>
	public class Hitscan_InstantPoint : HitscanBase
	{
		public override void OnCreated()
		{
			var start = HitscanInfo.Start;
			var shotPosition = HitscanInfo.Direction;
			var length = shotPosition.Value - start.Value;
			var direction = length.normalized;
			var distance = length.magnitude;

			// Check there is any obstacle
			var hits = Physics2D.RaycastAll
			(
				start,
				direction,
				distance,
				GlobalLayer.LAYER_RAYCAST_WALL_AREA_HIGH
			);

			if (hits.Length > 0)
			{
				var endPoint = hits.Last().point;
				createSyncEffect(endPoint);
				return;
			}

			var hitColliders = Physics2D.OverlapPointAll
			(
				shotPosition,
				GlobalLayer.LAYER_RAYCAST_HITBOX
			);

			List<RaycastHit2DInfo> raycastHitInfos = new();

			foreach (var c in hitColliders)
			{
				var hitbox = c.GetComponent<Hitbox>();
				raycastHitInfos.Add(new RaycastHit2DInfo(hitbox, shotPosition));
			}

			HitscanInfo = new HitscanInfo(HitscanInfo, raycastHitInfos);
			mHitscanHandler.Client_RequestPerform(HitscanID, HitscanInfo);
			createSyncEffect(shotPosition);

			OnRelease();
		}

		private void createSyncEffect(NetVector2 endPoint)
		{
			mHitscanHandler.EffectHandler.CreateSyncEffect
			(
				HitscanInfo.WeaponInfo.EffectType,
				endPoint,
				new Net2dRotation(Quaternion.identity)
			);
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
