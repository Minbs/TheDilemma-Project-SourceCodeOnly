using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileMapMesh 
{
    public Mesh Mesh { get; private set; }  
    private List<Vector3> mVertices = new();
    private List<Vector3> mNormals = new();
    private List<Vector2> mUvs = new();
    private List<int> mTriangles = new();

    public TileMapMesh()
	{
        Mesh = new Mesh();
        Mesh.name = $"Temp Mesh";
	}

    /// <summary> 메쉬 초기화 함수/// </summary>
    public void OnInitialize()
	{
        mVertices.Clear();
        mNormals.Clear();
        mUvs.Clear();
        mTriangles.Clear();
	}

    /// <summary> 한 면을 위한 정점과 UV, 삼각형 정보를 추가하는 함수/// </summary>
    public void CreateFace(Vector3[] vertices, Vector3 normal,Face face, float uvTileSize)
    {
        if (face.IsHidden)
            return;

        int verticesCount = mVertices.Count;
        mVertices.AddRange(vertices);
		

		Vector2 center = new Vector2(face.TextureCoord.x + 0.5f, face.TextureCoord.y + 0.5f);



        Vector2[] temp = new Vector2[4];
		// 유니티 UV Y축 반전을 위해 1을 뺌
		temp[0] = new Vector2((center.x + 0.5f) * uvTileSize, 1-(center.y + 0.5f) * uvTileSize); 
        temp[1] = new Vector2((center.x - 0.5f) * uvTileSize, 1-(center.y + 0.5f) * uvTileSize);
        temp[2] = new Vector2((center.x - 0.5f) * uvTileSize, 1-(center.y - 0.5f) * uvTileSize);
        temp[3] = new Vector2((center.x + 0.5f) * uvTileSize, 1-(center.y - 0.5f) * uvTileSize);

        for (int i = 0; i < 4; i++)
        {
            mUvs.Add(temp[i]);
        }


        mTriangles.Add(verticesCount + 0);
        mTriangles.Add(verticesCount + 1);
        mTriangles.Add(verticesCount + 2);
        mTriangles.Add(verticesCount + 0);
        mTriangles.Add(verticesCount + 2);
        mTriangles.Add(verticesCount + 3);
    }

    /// <summary> 데이터 최종 변경 이후 메쉬 재생성을 위한 함수 /// </summary>
    public void OnFinalize()
	{
        Mesh.Clear();
        Mesh.vertices = mVertices.ToArray();
        Mesh.normals = mNormals.ToArray();
        Mesh.uv = mUvs.ToArray();
        Mesh.triangles = mTriangles.ToArray();
        Mesh.RecalculateBounds();
		Mesh.RecalculateNormals();
		
	}
}
