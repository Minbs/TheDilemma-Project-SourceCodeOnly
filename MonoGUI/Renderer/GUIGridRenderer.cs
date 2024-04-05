using UnityEngine;
using UnityEngine.UI;

public class GUIGridRenderer : Graphic
{
	public float thickness;

	public Vector2Int gridSize = new Vector2Int(1, 1);

	float width;
	float height;
	float cellWidht;
	float cellHidht;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		Debug.Log("Draw");
		vh.Clear();

		cellWidht = width / gridSize.x;
		cellHidht = height / gridSize.y;

		int cellIndex = 0;

		for (int y = 0; y < gridSize.y; y++)
		{
			for (int x = 0; x < gridSize.x; x++)
			{
				drawCell(x, y, cellIndex, vh);
				cellIndex++;
			}
		}
	}

	private void drawCell(int x, int y, int index, VertexHelper vh)
	{
		float xPos = cellWidht * x;
		float yPos = cellHidht * y;

		UIVertex vertex = UIVertex.simpleVert;
		vertex.color = color;

		vertex.position = new Vector3(xPos, yPos);
		vh.AddVert(vertex);

		vertex.position = new Vector3(xPos, yPos + cellHidht);
		vh.AddVert(vertex);

		vertex.position = new Vector3(xPos + cellWidht, yPos + cellHidht);
		vh.AddVert(vertex);

		vertex.position = new Vector3(xPos + cellWidht, yPos);
		vh.AddVert(vertex);

		//vh.AddTriangle(0, 1, 2);
		//vh.AddTriangle(2, 3, 0);

		//이게 무엇일까?
		float widthSqr = thickness * thickness;
		float distanceSqr = widthSqr / 2f;
		float distanace = Mathf.Sqrt(distanceSqr);

		vertex.position = new Vector3(xPos + distanace, yPos + distanace);
		vh.AddVert(vertex);

		vertex.position = new Vector3(xPos + distanace, yPos + (cellHidht - distanace));
		vh.AddVert(vertex);

		vertex.position = new Vector3(xPos + (cellWidht - distanace), yPos + (cellHidht - distanace));
		vh.AddVert(vertex);

		vertex.position = new Vector3(xPos + (cellWidht - distanace), yPos + distanace);
		vh.AddVert(vertex);

		int offset = index * 8;

		vh.AddTriangle(offset, offset + 1, offset + 5);
		vh.AddTriangle(offset + 5, offset + 4, offset);

		vh.AddTriangle(offset + 1, offset + 2, offset + 6);
		vh.AddTriangle(offset + 6, offset + 5, offset + 1);

		vh.AddTriangle(offset + 2, offset + 3, offset + 7);
		vh.AddTriangle(offset + 7, offset + 6, offset + 2);

		vh.AddTriangle(offset + 3, offset, offset + 4);
		vh.AddTriangle(offset + 4, offset + 7, offset + 3);
	}
}
