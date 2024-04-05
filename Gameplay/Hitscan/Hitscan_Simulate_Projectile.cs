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
	public class Hitscan_Simulate_Projectile : HitscanBase
	{
		[field : SerializeField] public float Length { get; protected set; } = 2;
		[field : SerializeField] public float Speed { get; protected set; }
		[field : SerializeField] public float MaxDistance { get; protected set; }
		[field : SerializeField] public Vector2 Direction { get; protected set; }
		[field : SerializeField] public Vector2 Start { get; protected set; }
		private float mFlyDistance;

		private HashSet<NetObjectID> mHitset = new();

		public override void OnCreated()
		{
			Start = HitscanInfo.Start;
			MaxDistance = HitscanInfo.WeaponInfo.MaxDistance;
			Direction = HitscanInfo.Direction;
			Speed = HitscanInfo.WeaponInfo.Speed;

			mFlyDistance = 0;

			transform.right = Direction;
			transform.position = Start;
			mHitset.Clear();
		}

		public override void OnRelease()
		{
			mHitscanHandler.Release(this);
		}

		public void Update()
		{
			float deltaSpeed = Speed * Time.deltaTime;
			mFlyDistance += deltaSpeed;

			if (mFlyDistance > MaxDistance)
			{
				this.OnRelease();
				return;
			}

			transform.position = transform.position + (Direction * deltaSpeed).ToVector3();
			calculateHitscan();
		}

		private void calculateHitscan()
		{
			var hits = Physics2D.RaycastAll
			(
				transform.position,
				Direction,
				Length,
				GlobalLayer.LAYER_RAYCAST_HITBOX
			);

			if (hits.Length <= 0)
			{
				return;
			}

			int penetrateCount = HitscanInfo.WeaponInfo.PenetrateCount;
			List<RaycastHit2DInfo> raycastHitInfos = new();


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

				if (!mHitset.Add(entityHitbox.Entity.ID))
				{
					continue;
				}

				raycastHitInfos.Add(new RaycastHit2DInfo(entityHitbox, hit));

				penetrateCount--;
				if (penetrateCount <= 0)
				{
					break;
				}
			}

			HitscanInfo = new HitscanInfo(HitscanInfo, raycastHitInfos);
			mHitscanHandler.Client_RequestPerform(HitscanID, HitscanInfo);
			OnRelease();
		}

		public override void OnPerformed(HitscanInfo hitscanInfo)
		{
			OnRelease();
		}
	}
}
