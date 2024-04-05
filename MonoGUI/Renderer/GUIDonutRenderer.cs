using UnityEngine;
using UnityEngine.UI;

public class GUIDonutRenderer : Graphic
{
	private float mWidth;
	private float mHight;

	public float OutLineThickness;
	public int Sides;

	private UIVertex mVertex;
	private Vector3 mHalfSize;


	protected override void OnPopulateMesh(VertexHelper vh)
	{
		mWidth = rectTransform.rect.width;
		mHight = rectTransform.rect.height;

		mHalfSize = new Vector3(mWidth, mHight) * 0.5f;

		drawDonut(Sides, OutLineThickness, vh);
	}


	private void circumferencePoints(int sides, Vector2 radius,VertexHelper vh)
	{
		var stepPercent = 1.0f / sides;
		var circleRadian = 2.0f * Mathf.PI;
		var stepRadian = circleRadian * stepPercent;

		mVertex = UIVertex.simpleVert;
		mVertex.color = color;

		for (int i = 0; i < sides; i++)
		{
			float currentRadian = stepRadian * i;

			mVertex.position = rectTransform.anchoredPosition3D;
			mVertex.position += new Vector3(Mathf.Cos(currentRadian) * radius.x, Mathf.Sin(currentRadian) * radius.y);
			vh.AddVert(mVertex);
		}
	}

	private void drawDonut(int sides, float outLineThickness, VertexHelper vh)
	{
		vh.Clear();
		Vector2 outerRadius = mHalfSize;
		Vector2 innerRadius = new Vector2(mHalfSize.x - outLineThickness, mHalfSize.y - outLineThickness);
		circumferencePoints(sides, outerRadius, vh);
		circumferencePoints(sides, innerRadius, vh);

		drawHollowTriangles(vh);
	}

	private void drawHollowTriangles(VertexHelper vh)
	{
		int sides = vh.currentVertCount / 2;
		int triangleAmount = sides * 2;

		for(int i = 0; i < sides; i++)
		{
			int outerIndex = i;
			int innerIndex = i + sides;

			vh.AddTriangle(outerIndex, innerIndex, (i+1) % sides);

			vh.AddTriangle(outerIndex, sides + ((sides + i - 1) % sides), outerIndex + sides);
		}
	}
}
