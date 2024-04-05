using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
	public enum MapType : byte
	{
		None = 0,

		// Shop
		map_underground_stage_1 = 10,
		map_underground_stage_2,
		map_underground_stage_3,
		map_ending = 20,

		// Boss
		map_underground_boss_0,
	}

	public class MapHandler : MonoBehaviour
	{
		[Title("최초에 실행되는 이벤트입니다.")][field: SerializeField]
		public MapType Type;
		[Title("최초에 실행되는 이벤트입니다.")][field: SerializeField]
		public List<EntityTriggerEvent> InitialTriggerEvent { get; private set; } = new();
		[field : SerializeField]
		public List<Map_PointPlayerSpawn> PlayerSpawnPoints { get; private set; } = new();
		[field: SerializeField]
		public List<EntityTriggerEvent> EntityTriggerEvent { get; private set; } = new();
		[field: SerializeField]
		public List<LocatorBase> Locators { get; private set; } = new();

		public string Name => Type.ToString();

		public override string ToString() => Name;

		public GameplayManager GameplayManager { get; private set; }

		public void StartBy(GameplayManager gameplayManager)
		{
			GameplayManager = gameplayManager;

			List<EntityBase> empty = new();

			foreach (var t in EntityTriggerEvent)
			{
				t.InitializeBy(this);
			}

			foreach (var t in InitialTriggerEvent)
			{
				if (t != null)
				{
					t.OnDetected(empty);
				}
			}

			foreach (var locator in Locators)
			{
				locator.Initialize(gameplayManager);
				locator.StartLocator(GameplayManager.IsServerSide);
			}
		}

#if UNITY_EDITOR
		public void OnValidate()
		{
			PlayerSpawnPoints = GetComponentsInChildren<Map_PointPlayerSpawn>().ToList();
			EntityTriggerEvent = GetComponentsInChildren<EntityTriggerEvent>().ToList();
			Locators = GetComponentsInChildren<LocatorBase>().ToList();
		}
	#endif
	}
}
