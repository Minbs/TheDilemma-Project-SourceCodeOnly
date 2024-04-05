using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Gameplay
{
	public class MapController
	{
		public MapHandler CurrentMap { get; private set; }

		public void ChangeMap(MapType mapType, GameplayManager manager)
		{
			if (!GlobalServiceLocator
				.ResourcesService
				.GetServiceOrNull()
				.MapTable
				.TryGetValue(mapType, out var map))
			{
				Ulog.LogError(this, $"There is no such thing as map {CurrentMap}.");
				return;
			}

			string previousMap = CurrentMap == null ? "Null" : CurrentMap.Name;

			if (CurrentMap != null)
			{
				UnityEngine.Object.Destroy(CurrentMap.gameObject);
			}

			CurrentMap = UnityEngine.Object.Instantiate(map).GetComponent<MapHandler>();
			CurrentMap?.StartBy(manager);
			Ulog.Log(this, $"Change map \"{previousMap}\" to \"{CurrentMap}\"");
		}
	}
}
