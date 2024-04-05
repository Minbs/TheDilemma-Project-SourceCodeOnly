using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Proxy
{
	public static void FlipByDirection(Transform transform, Vector3 direction)
	{
		Vector3 tempScale = transform.localScale;
		tempScale.x = Mathf.Abs(tempScale.x);

		// 왼쪽을 바라봄
		if (Vector2.Dot(direction, Vector2.right) < 0)
		{
			tempScale.x *= -1;
		}

		transform.localScale = tempScale;
	}

	//public static void FlipByVelocity(T)
	//{

	//}
}
