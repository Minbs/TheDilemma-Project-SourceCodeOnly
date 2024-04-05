using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;

namespace Gameplay.Legacy
{
	[Obsolete]
	public class UserSessionData
	{
		public NetSessionID SessionID { get; private set; }
		public Entity_PlayerController PlayerEntity { get; private set; }
		public UserLoadoutData LoadoutData { get; private set;}

		public UserSessionData(NetSessionID sessionID)
		{
			SessionID = sessionID;
		}

		public void BindPlayer(Entity_PlayerController player)
		{
			PlayerEntity = player;
		}

		public void UnbindPlayer()
		{
			PlayerEntity = null;
		}

	}
}
