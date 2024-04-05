using System.Collections;
using System.Collections.Generic;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "EntityData", menuName = "Entity Data/Entity Data", order = 0)]
	public class EntityData : ScriptableObject
	{
		[field : SerializeField, Header("ID")]
		public NetObjectType EntityType { get; protected set; }

		[field : Header("Basic Data")]
		[field : SerializeField] public int HP { get; protected set; }
		[field : SerializeField] public float MoveSpeed { get; protected set; }
	}
}