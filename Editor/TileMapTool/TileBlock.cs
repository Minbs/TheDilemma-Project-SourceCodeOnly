using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

// ������ ���� �ű��
public class TileTextureSize
{
	public static readonly int Size = 32;
}

[System.Serializable]
public class TileBlock
{
	public Vector3Int Coord;
	public Face[] Faces = new Face[6];
}

[System.Serializable]
public struct Face 
{
	public Vector2 TextureCoord;
	public bool IsHidden;   // �ٸ� ���� ���� ����������

	public static bool operator ==(Face face1, Face face2) 
	{ 
		return face1.TextureCoord == face2.TextureCoord && face1.IsHidden == face2.IsHidden; 
	}
	public static bool operator !=(Face face1, Face face2) { return !face1.Equals(face2); }

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
