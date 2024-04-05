using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class PrefabPlaceEditor : MonoBehaviour
{
	public List<GameObject> PrefabsList { get; set; } = new();
	[SerializeField] public GameObject Preview { get; set; } = null;
	public Vector2 hoverGrid { get; set; } = Vector2.zero;

	public void CachePrefabs()
	{
		PrefabsList.Clear();
		var prefabs = AssetLoader.GetPrefabsFileFromPath(AssetLoader.DataPathOnEnvironment + @"\Prefabs\EditorTest\");
		if (Preview)
		{
			DestroyImmediate(Preview);
		}

		PrefabsList.AddRange(prefabs);
	}

	public GameObject PlacePrefab(GameObject gameObject, Vector3 position)
	{
		GameObject obj = PrefabUtility.InstantiatePrefab(gameObject, transform) as GameObject;// Instantiate(gameObject, position, Quaternion.identity);
		obj.transform.position = position;
		return obj;
	}

	public GameObject PlacePrefab(GameObject gameObject, Vector2Int position)
	{
		Vector2 pos = new Vector2(position.x, position.y);
		GameObject obj = PrefabUtility.InstantiatePrefab(gameObject, transform) as GameObject;
		obj.transform.position = pos;
		return obj;
	}

	public void ReplacePreview(GameObject gameObject)
	{
		if (Preview != null)
		{
			DestroyImmediate(Preview);
		}

		Preview = PrefabUtility.InstantiatePrefab(gameObject, transform) as GameObject;
		Preview.transform.SetAsFirstSibling();
		Preview.SetActive(false);
		Preview.gameObject.name = "Preview";

	}

	private void Update()
	{
		if (transform.hasChanged)
		{
			var childrenRenderer = transform.GetComponentsInChildren<Renderer>();
			foreach (var renderer in childrenRenderer)
			{
				int baseSortOrder = Global.RoundByDepthOrderOffset(renderer.transform);
				renderer.sortingOrder = baseSortOrder;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!Selection.transforms.Contains(transform))
		{
			if (Preview != null)
			{
				Preview.SetActive(false);
			}
			return;
		}

		if (Preview != null)
		{
			Preview.SetActive(true);
		}
		Gizmos.matrix = transform.localToWorldMatrix;
		int minX = -500;
		int maxX = 500;
		int minY = -500;
		int maxY = 500;
		Handles.color = new Color(0.5f, 0.5f, 0.5f, 1);
		for (int x = minX; x < maxX + 1; x++)
		{
			Vector3 pos1 = new Vector3(x, minY, 0);
			Vector3 pos2 = new Vector3(x, maxY, 0);

			Handles.DrawLine(pos1, pos2, 0.25f);
		}

		for (int y = minY; y < maxY + 1; y++)
		{
			Vector3 pos1 = new Vector3(minY, y, 0) * 0.75f;
			Vector3 pos2 = new Vector3(maxY, y, 0) * 0.75f;

			Handles.DrawLine(pos1, pos2, 0.25f);
		}

		Handles.color = Color.white;
		Handles.DrawSolidRectangleWithOutline(new Rect(hoverGrid, new Vector2(1, 0.75f)), new Color(0, 0, 0, 0), new Color(1, 1, 1, 1));
	}
}
#endif