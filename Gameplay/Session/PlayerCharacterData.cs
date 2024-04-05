using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	public class PlayerCharacterData
	{
		public CharacterType CharacterType;

		public int MaxHp;
		public float MoveSpeed;

		public EquipmentData Primary;
		public EquipmentData Secondary;
		public EquipmentData Auxillary;

		public EntityBasicData GetBasicEntityData()
		{
			return new EntityBasicData(FactionType.Human, MaxHp, MoveSpeed);
		}

		public float GetDamageMultiply()
		{
			return 1;
		}

		public int GetDamageReduce(int damage)
		{
			return damage;
		}

	}
}
