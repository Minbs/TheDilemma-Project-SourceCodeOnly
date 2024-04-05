using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum PaintMode
{
	Brush,
	Fill,
	Tile,
}

[CustomEditor(typeof(TileMeshEditor))]
public class TileMeshCustomEditor : Editor
{
	private Texture[] icons;
	public Texture BrushIconTexture;
	public Texture FillIconTexture;
	public Texture TileIconTexture;
	public TileMeshEditor TargetEditor { get { return (TileMeshEditor)target; } }
	public Vector3 OriginPosition { get { return TargetEditor.transform.position; } } // 메쉬 원점
																					  //public Texture texture;
	Event currentEvent;
	private PaintMode mPaintMode = PaintMode.Tile;
	private Rect PaintbarRect = new Rect(10, 70, 120, 450);

	// 선택한 타일의 좌표 및 면
	private class SelectedTile
	{
		public Vector3Int TileCoord;
		public Vector3 Face;
	}

	private SelectedTile mSingleSelection = null;
	private Face brush = new Face() { IsHidden = false };



	private void OnEnable()
	{
		Undo.undoRedoPerformed += OnUndoRedo;
	}

	private void OnDisable()
	{
		Undo.undoRedoPerformed -= OnUndoRedo;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Rebuild Mesh"))
		{
			TargetEditor.Rebuild();
		}

		if (GUILayout.Button("Save Mesh"))
		{
			string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", name, "asset");
			if (string.IsNullOrEmpty(path)) return;

			path = FileUtil.GetProjectRelativePath(path);

			Mesh meshToSave = Instantiate(TargetEditor.mTileMapMesh.Mesh);
			MeshUtility.Optimize(meshToSave);

			AssetDatabase.CreateAsset(meshToSave, path);
			AssetDatabase.SaveAssets();

		}
	}

	private GUIStyle style = new GUIStyle() { };
	private void OnSceneGUI()
	{

		currentEvent = Event.current;
		var invokeRepaint = false;
		GUIStyle guiStyle = new GUIStyle();

		Handles.BeginGUI();
		{
			icons = new Texture[3];
			icons[0] = BrushIconTexture;
			icons[1] = FillIconTexture;
			icons[2] = TileIconTexture;
			GUI.backgroundColor = Color.grey;
			PaintbarRect = GUI.Window(0, PaintbarRect, PaintingWindow, "");
		}
		Handles.EndGUI();

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		Tools.current = Tool.None;
		PaintingMode();

		// 마우스 오버한 타일 선택
		if (currentEvent.type.Equals(EventType.MouseMove)
			|| currentEvent.type.Equals(EventType.MouseDrag)
			|| currentEvent.type.Equals(EventType.MouseDown))
		{
			var next = GetSelectionAt(currentEvent.mousePosition);

			mSingleSelection = next;
			invokeRepaint = true;

		}

		// 갱신된 내용 다시 그림
		if (invokeRepaint)
		{
			Repaint();
		}
		Selection.activeGameObject = TargetEditor.transform.gameObject;
	}

	/// <summary> 마우스 오버한 박스 리턴/// </summary>
	private SelectedTile GetSelectionAt(Vector2 mousePosition)
	{
		var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
		var hits = Physics.RaycastAll(ray);

		foreach (var hit in hits)
		{
			var other = hit.collider.gameObject.GetComponent<TileMeshEditor>();
			if (other == TargetEditor)
			{
				var center = hit.point - hit.normal * 0.5f;

				return new SelectedTile()
				{
					TileCoord = (center - OriginPosition).Floor(),
					Face = hit.normal
				};
			}
		}

		return null;
	}

	#region BrushMode
	private void PaintingMode()
	{
		if (mSingleSelection == null)
			return;

		var block = TargetEditor.GetTile(mSingleSelection.TileCoord);
		var pressed = (currentEvent.type == EventType.MouseDown || currentEvent.type.Equals(EventType.MouseDrag)) && currentEvent.button == 0; // 왼쪽 클릭 및 드래그


		if (mPaintMode == PaintMode.Brush && block != null && pressed)
		{
			TrySetBlockFace(block, mSingleSelection.Face, brush);
			TargetEditor.Rebuild();
		}
		else if (mPaintMode == PaintMode.Fill && block != null && pressed)
		{
			TryFillBlockFaces(block, mSingleSelection.Face, brush);
			TargetEditor.Rebuild();
		}
		else if (mPaintMode == PaintMode.Tile && currentEvent.type == EventType.MouseDown)
		{
			BuildingMode();
		}
	}

	private void BuildingMode()
	{
		var from = mSingleSelection.TileCoord;
		var tile = mSingleSelection.TileCoord + mSingleSelection.Face.Int();

		if (currentEvent.button == 0)
		{
			TargetEditor.Create(tile, from);
		}
		else if (currentEvent.button == 1)
		{
			TargetEditor.Destroy(from);
		}

		TargetEditor.Rebuild();
		Repaint();

	}
	/// <summary>
	/// 타일의 텍스처를 브러쉬의 텍스처로 변경 	
	/// </summary>
	private bool TrySetBlockFace(TileBlock block, Vector3 normal, Face brush)
	{
		Undo.RecordObject(target, "SetBlockFaces");
		var rows = TargetEditor.Texture.height / TileTextureSize.Size;
		brush.TextureCoord = new Vector2(brush.TextureCoord.x, rows - 1 - brush.TextureCoord.y); // 유니티 UV Y값 뒤집어야함

		for (int i = 0; i < TileMeshEditor.Faces.Length; i++)
		{
			if (Vector3.Dot(normal, TileMeshEditor.Faces[i]) > 0.8f)
			{
				if (!brush.IsHidden)
				{
					if (brush != block.Faces[i])
					{
						block.Faces[i] = brush;
						return true;
					}
				}
				else if (!block.Faces[i].IsHidden)
				{
					block.Faces[i].IsHidden = true;
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// 인접한 타일의 텍스처를 브러쉬의 텍스처로 변경
	/// </summary>
	/// <param name="block"></param>
	/// <param name="normal"></param>
	/// <param name="brush"></param>
	/// <returns></returns>
	private bool TryFillBlockFaces(TileBlock block, Vector3 normal, Face brush)
	{
		Undo.RecordObject(target, "FillBlockFaces");
		var rows = TargetEditor.Texture.height / TileTextureSize.Size;
		brush.TextureCoord = new Vector2(brush.TextureCoord.x, rows - 1 - brush.TextureCoord.y); // 유니티 UV Y값 뒤집어야함
		int normalIndex = 0;
		for (int i = 0; i < TileMeshEditor.Faces.Length; i++)
		{
			if (Vector3.Dot(normal, TileMeshEditor.Faces[i]) > 0.8f)
			{
				normalIndex = i;
				break;
			}
		}
		Vector2 targetTexture = TargetEditor.GetTile(mSingleSelection.TileCoord).Faces[normalIndex].TextureCoord;

		if (PaintNearFaces(block.Coord, normalIndex, targetTexture, brush))
		{
			Repaint();
		}

		return true;
	}

	/// <summary>
	/// 인접한 타일의 텍스처를 변경하는 함수입니다
	/// </summary>
	/// <returns> 해당 타일의 텍스처를 변경했다면 true 반환 </returns>
	private bool PaintNearFaces(Vector3Int coord, int normalIndex, Vector2 targetTexture, Face brush)
	{
		TileBlock block = TargetEditor.GetTile(coord);

		if (block == null
			|| block.Faces[normalIndex] == brush
			|| block.Faces[normalIndex].TextureCoord != targetTexture
			|| block.Faces[normalIndex].IsHidden)
		{
			return false;
		}

		block.Faces[normalIndex] = brush;

		// 그려줄 타일의 위치 정하기
		int nearX = (normalIndex == (int)Normal.Left || normalIndex == (int)Normal.Right) ? 0 : 1;
		int nearY = (normalIndex == (int)Normal.Up || normalIndex == (int)Normal.Down) ? 0 : 1;
		int nearZ = (normalIndex == (int)Normal.Forward || normalIndex == (int)Normal.Back) ? 0 : 1;

		// 인접한 타일의 텍스처 그려주기
		PaintNearFaces(coord + new Vector3Int(nearX, 0, 0), normalIndex, targetTexture, brush);
		PaintNearFaces(coord + new Vector3Int(-nearX, 0, 0), normalIndex, targetTexture, brush);
		PaintNearFaces(coord + new Vector3Int(0, nearY, 0), normalIndex, targetTexture, brush);
		PaintNearFaces(coord + new Vector3Int(0, -nearY, 0), normalIndex, targetTexture, brush);
		PaintNearFaces(coord + new Vector3Int(0, 0, nearZ), normalIndex, targetTexture, brush);
		PaintNearFaces(coord + new Vector3Int(0, 0, -nearZ), normalIndex, targetTexture, brush);

		return true;
	}
	Vector2 scrollPos = Vector2.zero;

	/// <summary> 타일 팔레트 창/// </summary>
	void PaintingWindow(int id)
	{
		const int leftPadding = 10;
		const int topPadding = 30;
		const int width = 300;

		mPaintMode = (PaintMode)GUI.Toolbar(new Rect(leftPadding, topPadding + 25, 70, 30), (int)mPaintMode, icons);

		Vector2 center = new Vector2(brush.TextureCoord.x + 0.5f, brush.TextureCoord.y + 0.5f);

		var coords = new Rect((center.x - 0.5f) * TargetEditor.UVTileSize.x, (center.y - 0.5f) * TargetEditor.UVTileSize.y, TargetEditor.UVTileSize.x, TargetEditor.UVTileSize.y);
		GUI.DrawTextureWithTexCoords(new Rect(leftPadding, topPadding + 65, 40, 40), TargetEditor.Texture, coords); // 현재 선택한 텍스처

		if (TargetEditor.Texture == null)
		{
			GUI.Label(new Rect(leftPadding, topPadding + 120, width, 80), "Material이 필요합니다");
		}
		else
		{
			var columns = 2;
			var rows = (TargetEditor.Texture.height / TileTextureSize.Size) * (TargetEditor.Texture.width / TileTextureSize.Size) / columns;
			var tileWidth = 40;
			var tileHeight = 40;

			EditorGUI.DrawRect(new Rect(leftPadding - 1, topPadding + 120 - 1, tileWidth * columns + 15 + 2, 250 + 2), Color.black);
			scrollPos = GUI.BeginScrollView(new Rect(leftPadding, topPadding + 120, tileWidth * columns + 15, 250), scrollPos, new Rect(leftPadding, topPadding + 120, tileWidth * columns, tileHeight * rows), false, true);
			for (int x = 0; x < columns; x++)
			{
				for (int y = 0; y < rows; y++)
				{
					var rect = new Rect(leftPadding + x * tileWidth, topPadding + 120 + y * tileHeight, tileWidth, tileHeight);
					var rowCount = (TargetEditor.Texture.width / TileTextureSize.Size);
					var tile = new Vector2Int((x + y * columns) % rowCount, rowCount - 1 - (x + y * columns) / rowCount);
					if (DrawPaletteTile(rect, tile, brush.TextureCoord == tile)) // 선택한 타일이 있다면
					{
						brush.TextureCoord = tile;
						brush.IsHidden = false;
					}
				}
			}
			GUI.EndScrollView();
		}



		if (currentEvent.type == EventType.MouseMove || currentEvent.type == EventType.MouseDown)
			Repaint();

		GUI.DragWindow();
	}

	/// <summary>
	/// 타일 텍스처 이미지 띄우기 및 선택
	/// </summary>
	/// <returns> 타일 선택에 성공했다면 true 반환</returns>
	private bool DrawPaletteTile(Rect rect, Vector2Int? tile, bool selected)
	{
		var mouseOver = !selected && currentEvent.mousePosition.x > rect.x && currentEvent.mousePosition.y > rect.y && currentEvent.mousePosition.x < rect.xMax && currentEvent.mousePosition.y < rect.yMax;
		var pressed = mouseOver && currentEvent.type == EventType.MouseDown && currentEvent.button == 0;

		if (mouseOver)
		{
			EditorGUI.DrawRect(rect, Color.blue);
		}
		else if (selected)
		{
			EditorGUI.DrawRect(rect, Color.white);
		}

		if (tile.HasValue)
		{
			var coords = new Rect(tile.Value.x * TargetEditor.UVTileSize.x, tile.Value.y * TargetEditor.UVTileSize.y, TargetEditor.UVTileSize.x, TargetEditor.UVTileSize.y);
			GUI.DrawTextureWithTexCoords(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), TargetEditor.Texture, coords);
		}


		if (pressed)
			currentEvent.Use();

		return pressed;
	}
	#endregion

	#region BuildingMode

	#endregion

	/// <summary> 취소 이벤트 콜백 함수 /// </summary>
	void OnUndoRedo()
	{
		var editor = target as TileMeshEditor;
		editor.RebuildBlockMap();
		editor.Rebuild();
	}
}
