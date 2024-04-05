using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(PrefabPlaceEditor))]
public class PrefabPlaceCustomEditor : Editor
{
	public PrefabPlaceEditor TargetEditor { get { return target as PrefabPlaceEditor; } }
	public Event currentEvent { get; set; }
	private int mode { get; set; }
	private int selectedObjectIndex;
	Dictionary<Vector2, GameObject> PlacedObjectsTable { get; set; } = new();
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		drawGUI();

		mode = GUI.SelectionGrid(EditorGUILayout.GetControlRect(false, 30), mode, new string[] { "Draw", "Erase" }, 2);
		;
		if (GUILayout.Button("Cache"))
		{
			TargetEditor.CachePrefabs();
		}
	}

	private void OnSceneGUI()
	{
		currentEvent = Event.current;
		Handles.BeginGUI();
		OnSelectionMouse();
		Tools.current = Tool.None;
		Handles.EndGUI();

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
	}

	Vector2 scrollPosition = Vector2.zero;
	int columnCount = 4;
	private void drawGUI()
	{
		if (TargetEditor.PrefabsList.Count == 0)
			return;


		//EditorGUILayout.BeginHorizontal();
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.ExpandWidth(true), GUILayout.Height(200));
		int rowCount = Mathf.CeilToInt((float)TargetEditor.PrefabsList.Count / columnCount);


		for (int y = 0; y < rowCount; y++)
		{
			GUILayout.BeginHorizontal();
			for (int x = 0; x < columnCount; x++)
			{
				if (y * columnCount + x > TargetEditor.PrefabsList.Count - 1)
					break;

				var img = AssetPreview.GetAssetPreview(TargetEditor.PrefabsList[y * columnCount + x]);
				if (img != null)
				{
					if (GUILayout.Button(img, GUILayout.Width(50), GUILayout.Height(50), GUILayout.ExpandWidth(true), GUILayout.ExpandWidth(true)))
					{
						TargetEditor.ReplacePreview(TargetEditor.PrefabsList[y * columnCount + x]);
						selectedObjectIndex = y * columnCount + x;
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.EndScrollView();
		//EditorGUILayout.EndHorizontal();
	}


	private void OnSelectionMouse()
	{
		Vector2 pos = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition).GetPoint(.1f).ToVector2();
		float y = 0;
		while (true)
		{
			if(pos.y >= 0)
			{
				y += 0.75f;
				if(y >= pos.y)
				{
					break;
				}
			}
			else
			{
				y -= 0.75f;
				if (y <= pos.y)
				{
					break;
				}
			}
		}



		if (mode == 0)
		{
			TargetEditor.hoverGrid = new Vector2(-1000, -1000);
		}
		else if (mode==1)
		{
			if (TargetEditor.Preview != null)
			{
				TargetEditor.Preview.gameObject.SetActive(false);
			}
			TargetEditor.hoverGrid = new Vector2(Mathf.FloorToInt(pos.x), y);
		}

		if(TargetEditor.Preview != null)
		{
			TargetEditor.Preview.transform.position = pos;

			TargetEditor.Preview.transform.position = new Vector3(Mathf.FloorToInt(pos.x), y, 0);
			if ((currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag) && currentEvent.button == 0 && mode == 0)
			{
				Vector2 coord = new Vector2(Mathf.FloorToInt(pos.x), y);

				if(PlacedObjectsTable.ContainsKey(coord))
				{
					DestroyImmediate(PlacedObjectsTable[coord]);
					PlacedObjectsTable.Remove(coord);
				}
				
				GameObject obj = TargetEditor.PlacePrefab(TargetEditor.PrefabsList[selectedObjectIndex], TargetEditor.Preview.transform.position);
				Undo.RecordObject(obj, "PlaceObject");
				Undo.RegisterCreatedObjectUndo(obj, "Create New");
				PlacedObjectsTable.Add(coord, obj);
			}

		}

		if ((currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag) && currentEvent.button == 0 && mode == 1)
		{

			Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
			RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 100);

			foreach (var hit in hits)
			{
				if (hit.collider.gameObject == null || hit.collider.gameObject == TargetEditor.Preview.gameObject || hit.collider.gameObject.layer != GlobalLayer.LAYER_WALL_AREA_HIGH)
					continue;

				Undo.DestroyObjectImmediate(hit.transform.gameObject);
			}
		}
	}
}
#endif