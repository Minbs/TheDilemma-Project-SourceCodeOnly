using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
	public class Map_PointPlayerSpawn : MonoBehaviour
	{
		public Vector3 SpawnPosition => transform.position;

#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			Handles.matrix = transform.localToWorldMatrix;
			Handles.color = Color.green;
			Handles.DrawSolidDisc(Vector3.zero, Vector3.back, 0.3F);

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			Handles.Label(Vector3.zero, typeof(Map_PointPlayerSpawn).Name, style);
		}
#endif
	}
}
