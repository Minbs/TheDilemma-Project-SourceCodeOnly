using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(PhysicsViewer))]
public class PhysicsCustomViewer : Editor
{
	public PhysicsViewer TargetEditor { get { return (PhysicsViewer)target; } }
	private Event mCurrentEvent;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(GUILayout.Button("Physics Info"))
		{
			TargetEditor.GetPhysicsInfo();
		}
	}

	private void OnSceneGUI()
	{
		mCurrentEvent = Event.current;

		Handles.BeginGUI();
		{
			// 모드 만들기
		}
		Handles.EndGUI();
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		drawColliders();
		Tools.hidden = true;
		Selection.selectionChanged += () => Tools.hidden = false;
		Repaint();
	}


	private void drawColliders()
	{
		foreach( var collider in TargetEditor.Colliders)
		{
			if(collider.GetType() == typeof(BoxCollider2D)) drawBoxCollider((BoxCollider2D)collider);
			if (collider.GetType() == typeof(CapsuleCollider2D)) drawCapsuleCollider((CapsuleCollider2D)collider);
			if (collider.GetType() == typeof(CircleCollider2D)) drawCircleCollider((CircleCollider2D)collider);
		}
	}

	private void drawBoxCollider(BoxCollider2D collider)
	{
		Rect rect = new Rect(collider.bounds.min, collider.size);
		Vector2 rightDown = rect.min;
		rightDown.x += rect.width;
		Vector2 leftUp = rect.max;
		leftUp.x -= rect.width;

		Handles.color = Color.cyan;
		Handles.DrawLine(rect.min, rightDown, 1);
		Handles.DrawLine(rect.min, leftUp, 1);
		Handles.DrawLine(rect.max, rightDown, 1);
		Handles.DrawLine(rect.max, leftUp, 1);
		
		Repaint();
	}

	private void drawCapsuleCollider(CapsuleCollider2D collider)
	{
		Handles.color = Color.red;
		var offsetX = collider.bounds.size.x / 2;
		var offsetY = collider.bounds.size.y / 4;
		Handles.DrawWireArc(collider.bounds.center + Vector3.up * offsetY, Vector3.back, Vector3.left, 180, collider.bounds.size.x / 2);
		Handles.DrawLine(collider.bounds.center + new Vector3(offsetX, offsetY), collider.bounds.center + new Vector3(offsetX, -offsetY));
		Handles.DrawLine(collider.bounds.center + new Vector3(-offsetX, offsetY), collider.bounds.center + new Vector3(-offsetX, -offsetY));
		Handles.DrawWireArc(collider.bounds.center + Vector3.down * offsetY, Vector3.back, Vector3.left, -180, collider.bounds.size.x / 2);
		Repaint();
	}

	private void drawCircleCollider(CircleCollider2D collider)
	{
		Handles.color = Color.blue;
		Handles.DrawWireDisc(collider.bounds.center, Vector3.back, collider.bounds.size.x / 2);

	}
}
#endif