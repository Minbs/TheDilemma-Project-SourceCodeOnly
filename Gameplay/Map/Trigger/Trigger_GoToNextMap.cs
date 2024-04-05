using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gameplay;

namespace Gameplay
{
	public class Trigger_GoToNextMap : EntityTriggerEvent
	{
		public override void OnDetected(IList<EntityBase> entityBases)
		{
			Handler.GameplayManager.EventManager.OnPlayerReachedEndpoint();
		}

		public override void OnUndetected()
		{
			Handler.GameplayManager.EventManager.OnNoPlayerDetectedAtEndPoint();
		}
	}
}
