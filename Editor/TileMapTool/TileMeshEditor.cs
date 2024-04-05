using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Normal
{
	Up,
	Down,
	Left,
	Right,
	Forward,
	Back
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class TileMeshEditor : MonoBehaviour
{
    public static Vector3Int[] Faces = new Vector3Int[6]
    {
		Vector3Int.up,
		Vector3Int.down,
		Vector3Int.left,
		Vector3Int.right,
		Vector3Int.forward,
		Vector3Int.back
    };

    public List<TileBlock> mBlocks;
    private Dictionary<Vector3Int, TileBlock> mTilePositionMap;
    public MeshRenderer mMeshRenderer;
    private MeshFilter mMeshFiler;
    private MeshCollider mMeshCollider;
    public TileMapMesh mTileMapMesh;
    public Texture Texture
    {
        get
        {
            var material = mMeshRenderer.sharedMaterial;
            if (material != null)
                return material.mainTexture;
            return null;
        }
    }
    public Vector2 UVTileSize
    {
        get
        {
            if (Texture != null)
                return new Vector2(1f / (Texture.width / TileTextureSize.Size), 1f / (Texture.height / TileTextureSize.Size));
            return Vector2.one;
        }
    }

	private void OnEnable()
	{
        mMeshFiler = GetComponent<MeshFilter>();
        mMeshRenderer = GetComponent<MeshRenderer>();
        mMeshCollider = GetComponent<MeshCollider>();

        if(mTileMapMesh == null)
		{
            mTileMapMesh = new();
            mMeshFiler.sharedMesh = mTileMapMesh.Mesh;
            mMeshCollider.sharedMesh = mTileMapMesh.Mesh;
        }

        if (mTilePositionMap == null)
        {
            RebuildBlockMap();
        }

        // make initial cells
        if (mBlocks == null)
        {
            mBlocks = new();
            for (int x = -4; x < 4; x++)
                for (int z = -4; z < 4; z++)
                    Create(new Vector3Int(x, 0, z));
        }

        Rebuild();
    }

    /// <summary>
    /// 블럭을 생성하는 함수입니다
    /// </summary>
    /// <param name="at"> 블럭을 생성할 위치 </param>
    /// <param name="from"> 새로운 블럭을 생성하기 위해 선택한 블럭의 위치</param>
    public TileBlock Create(Vector3Int at, Vector3Int? from = null)
    {
        TileBlock block;
        if (!mTilePositionMap.TryGetValue(at, out block))
        {
            block = new TileBlock();
            block.Coord = at;
            mBlocks.Add(block);
            mTilePositionMap.Add(at, block);

            if (from != null)
            {
                var before = GetTile(from.Value);
                if (before != null)
                    for (int i = 0; i < Faces.Length; i++)
                        block.Faces[i] = before.Faces[i];
                
            }
        }

        return block;
    }

    /// <summary>
    /// 해당 위치에 있는 블럭을 제거하는 함수입니다
    /// </summary>
    /// <param name="at"> 블럭을 제거할 위치</param>
    public void Destroy(Vector3Int at)
    {
        TileBlock block;
        if (mTilePositionMap.TryGetValue(at, out block))
        {
            mTilePositionMap.Remove(at);
            mBlocks.Remove(block);
        }
    }

    public TileBlock GetTile(Vector3Int at)
    {
        TileBlock block;
        if (mTilePositionMap.TryGetValue(at, out block))
            return block;
        return null;
    }

    /// <summary> 타일의 위치을 키로 가지는 타일의 해쉬맵을 갱신하는 함수입니다 /// </summary>
    public void RebuildBlockMap()
    {
        mTilePositionMap = new();
        if (mBlocks != null)
            foreach (var cell in mBlocks)
                mTilePositionMap.Add(cell.Coord, cell);
    }

    /// <summary> 메쉬를 초기화하고 재생성하는 함수입니다 /// </summary>
    public void Rebuild()
    {
        mTileMapMesh.OnInitialize();

        foreach (var block in mBlocks)
        {
            var origin = new Vector3(block.Coord.x + 0.5f, block.Coord.y + 0.5f, block.Coord.z + 0.5f);

            for (int i = 0; i < Faces.Length; i++)
            {
                var normal = new Vector3Int((int)Faces[i].x, (int)Faces[i].y, (int)Faces[i].z);
                if (GetTile(block.Coord + normal) == null)
                    BuildFace(origin, normal, block.Faces[i]);
            }
        }

        mTileMapMesh.OnFinalize();

        mMeshFiler.sharedMesh = mTileMapMesh.Mesh;
        mMeshCollider.sharedMesh = mTileMapMesh.Mesh;
    }

    private void BuildFace(Vector3 center, Vector3 normal, Face face)
    {
        var up = Vector3.down;
        if (normal.y != 0) // 윗면, 아랫면일 때
            up = Vector3.back;

        var front = center + normal * 0.5f; // 앞면의 위치
        var cross1 = Vector3.Cross(normal, up); // 노말 벡터와 업 벡터에 수직인 벡터
        var cross2 = Vector3.Cross(cross1, normal);

        var v1 = front + (-cross1 + cross2) * 0.5f;
        var v2 = front + (cross1 + cross2) * 0.5f;
        var v3 = front + (cross1 + -cross2) * 0.5f;
        var v4 = front + (-cross1 + -cross2) * 0.5f;

        Vector3[] vertices = { v1, v2, v3, v4 };
        mTileMapMesh.CreateFace(vertices, normal ,face, UVTileSize.x);
    }
}
