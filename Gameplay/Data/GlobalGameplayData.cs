using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
	public static class GlobalGameplayData
	{
		public const int WEAPON_SLOT_COUNT = 2;
		public const int ITEM_SLOT_COUNT = 3;

		public const float FRIENDLY_FIRE_RATIO = 0.25F;
		public const float MOA_DISTANCE = 10.0f;

		/// <summary>월드의 기저 비율입니다. y를 x로 나눈 비 입니다.</summary>
		public const float WorldBasisRatio = 24.0F / 32.0F;

		/// <summary>맵	변경시 화면 전환 초 입니다.</summary>
		public const float MapChangeFadeoutSec = 1.0f;
	}
}
