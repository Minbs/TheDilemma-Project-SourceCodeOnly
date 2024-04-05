using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Utils;
using KaNet.Synchronizers;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay
{

	public class Locator_Entity : LocatorBase
	{
		[Title("Faction Match Condition")] public List<FactionType> FactionMatchList = new();
		[Title("Type Match Condition")] public List<NetObjectType> TypeMatchList = new();
		[Title("Detected Events")] public List<EntityTriggerEvent> TriggerEvents = new();

		public bool ShouldFactionMatchAll => FactionMatchList.Count <= 0;
		public bool ShouldTypeMatchAll => TypeMatchList.Count <= 0;

		private Coroutine mDetection;

		public override void StartLocator(bool isCurrentlyServerSide)
		{
			if (IsServerSide && !isCurrentlyServerSide)
			{
				return;
			}

			gameObject.SetActive(true);

			if (mDetection != null)
			{
				StopCoroutine(mDetection);
			}

			mDetection = StartCoroutine(detectArea());
		}

		public override void StopLocator()
		{
			if (mDetection != null)
			{
				StopCoroutine(mDetection);
			}

			gameObject.SetActive(false);
		}

		private List<EntityBase> mDetectedList = new();
		private bool mIsPreDetected = false;

		private IEnumerator detectArea()
		{
			while (true)
			{
				yield return new WaitForSeconds(DetectTick);

				mDetectedList.Clear();

				var hits = Physics2D.BoxCastAll
				(
					transform.position,
					transform.localScale, 
					transform.rotation.eulerAngles.z, 
					Vector2.zero,
					0,
					GlobalLayer.LAYER_RAYCAST_ENTITY_AREA
				);

				foreach (var h in hits)
				{
					if (!h.collider.TryGetComponent<EntityBase>(out var no))
					{
						continue;
					}

					if (!ShouldFactionMatchAll && !FactionMatchList.Contains(no.Faction))
					{
						continue;
					}

					if (!ShouldTypeMatchAll && !TypeMatchList.Contains(no.Type))
					{
						continue;
					}

					mDetectedList.Add(no);
				}

				if (!mDetectedList.IsEmpty())
				{
					foreach (var t in TriggerEvents)
					{
						t.OnDetected(mDetectedList);
						mIsPreDetected = true;
					}

					if (IsSingleUse)
					{
						this.StopLocator();
					}
				}
				else
				{
					if (mIsPreDetected == true)
					{
						mIsPreDetected = false;

						foreach (var t in TriggerEvents)
						{
							t.OnUndetected();
						}
					}
				}
			}
		}

#if UNITY_EDITOR
		[Title("Gizmo Setting")]
		public Color GizmoColor = Color.white;

		public void OnDrawGizmos()
		{
			Handles.matrix = transform.localToWorldMatrix;
			Handles.DrawSolidRectangleWithOutline
			(
				new Rect(Vector2.one * -0.5f, Vector2.one), 
				GizmoColor,
				GizmoColor * 0.5f
			);
		}

		[Button]
		public void BindTriggerNearBy()
		{
			TriggerEvents = new(transform.parent.GetComponentsInChildren<EntityTriggerEvent>());
		}
#endif
	}
}
