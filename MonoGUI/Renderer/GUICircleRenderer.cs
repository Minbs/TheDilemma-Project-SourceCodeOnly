using UnityEngine;
using UnityEngine.UI;

public class GUICircleRenderer : Graphic
{
	private float mWidth;
	private float mHight;

	public int Sides;

	private UIVertex mVertex;

	private Vector3 mHalfSize;


	protected override void OnPopulateMesh(VertexHelper vh)
	{
		mWidth = rectTransform.rect.width;
		mHight = rectTransform.rect.height;

		mHalfSize = new Vector3(mWidth, mHight) * 0.5f;

		drawFilled(Sides, mHalfSize, vh);
	}

	private void drawFilled(int sides, Vector2 radius, VertexHelper vh)
	{
		vh.Clear();
		circumferencePoints(sides, radius, vh);
		drawFilledTriangles(sides, vh);
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

			mVertex.position = mHalfSize;
			mVertex.position += new Vector3(Mathf.Cos(currentRadian) * radius.x, Mathf.Sin(currentRadian) * radius.y);
			vh.AddVert(mVertex);
		}
	}

	private void drawFilledTriangles(int sides, VertexHelper vh)
	{
		int triangleAmount = vh.currentVertCount - 2;

		for (int i = 0; i < triangleAmount; i++)
		{
			vh.AddTriangle(0, i + 2, i + 1);
		}
	}

}
