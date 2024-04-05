using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Gameplay;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEditor;

/// <summary>서버가 제어하고 동기화되는 Hitscan입니다.</summary>
public class NetHitscan_RectArea : NetHitscanBase
{
	public override NetObjectType Type => NetObjectType.Hitscan_Area;

#if UNITY_EDITOR
	[Title("Gizmo Setting")]
	public Color GizmoColor = Color.white;

	public void OnDrawGizmos()
	{
		Handles.matrix = transform.localToWorldMatrix;
		Handles.DrawSolidRectangleWithOutline
		(
			new Rect(Vector2.one * -0.5f, Vector2.one),
			GizmoColor,
		GizmoColor * 0.5f
		);
	}
#endif
}
