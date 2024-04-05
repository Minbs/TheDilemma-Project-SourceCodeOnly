using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMouseRay : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer sprite;


	public void OnAction()
	{
		sprite.color = Color.red;
	}
}
