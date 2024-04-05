using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
	public enum EnemySpawnType
	{
		None,

		/// <summary>숨겨진 영역에서 생성됩니다.</summary>
		Hidden,
		/// <summary>공개된 장소에서 생성됩니다.</summary>
		Public,
	}

	public class Trigger_PointEntitySpawn : EntityTriggerEvent
	{
		public Vector3 SpawnPosition
		{
			get
			{
				var spawnPos = transform.position;
				spawnPos.z = 1;
				return spawnPos;
			}
		}

		[field : SerializeField] public EnemySpawnType EnemySpawnType { get; private set; }
		[field : SerializeField] public NetObjectType SpawnType { get; private set; }
		[Title("객체 기본 정보")]
		public FactionType Faction = FactionType.Creature;

		public override void OnDetected(IList<EntityBase> entityBases)
		{
			GameplayManager.EntityService.Server_SpawnEntity(SpawnType, SpawnPosition, Faction);
		}

		public override void OnUndetected()
		{
		}

#if UNITY_EDITOR
		[Title("Gizmo Setting")]
		public Color GizmoColor = Color.blue;

		public void OnDrawGizmos()
		{
			Handles.matrix = transform.localToWorldMatrix;
			Handles.color =	GizmoColor;
			Handles.DrawSolidDisc(Vector3.zero, Vector3.back, 0.3F);

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			Handles.Label(Vector3.zero, SpawnType.ToString(), style);
		}
#endif
	}
}
