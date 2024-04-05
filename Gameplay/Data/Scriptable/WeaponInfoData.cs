using System.Collections;
using System.Collections.Generic;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "WeaponInfoData", menuName = "Entity Data/Weapon Info Data", order = 0)]
	public class WeaponInfoData : ScriptableObject
	{
		[field: Header("ID")]
		/// <summary>무기의 타입입니다.</summary>
		[field: SerializeField] public EquipmentType WeaponType { get; protected set; }
		/// <summary>이펙트 타입입니다.</summary>
		[field: SerializeField] public EffectType EffectType { get; protected set; }
		/// <summary>히트스켄 타입입니다.</summary>
		[field: SerializeField] public HitscanType HitscanType { get; protected set; }

		[field: Header("Data")]
		/// <summary>관통 횟수입니다.</summary>
		[field: SerializeField] public byte PenetrateCount { get; protected set; }
		/// <summary>최대 발사 거리입니다.</summary>
		[field: SerializeField] public float MaxDistance { get; protected set; }
		/// <summary>무기의 데미지입니다.</summary>
		[field : SerializeField] public short Damage { get; protected set; }
		/// <summary>발사한 공격의 속도입니다.</summary>
		[field: SerializeField] public float Speed { get; protected set; }
		/// <summary>아군 오사 허용 여부입니다.</summary>
		[field: SerializeField] public bool AllowFriendlyFire { get; protected set; }
		
		public WeaponInfo GetWeaponInfo()
		{
			return new WeaponInfo
			(
				WeaponType,
				EffectType,
				HitscanType,
				PenetrateCount,
				MaxDistance,
				Damage,
				Speed,
				AllowFriendlyFire
			);
		}
	}
}