using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Test : MonoBehaviour
{
	public SpriteRenderer[] Renderers = new SpriteRenderer[0];

	public void OnEnable()
	{
		StartCoroutine(onCreated());
	}

	IEnumerator onCreated()
	{
		for (float alpha = 1; alpha >= 0; alpha -= Time.deltaTime)
		{
			foreach (var r in Renderers)
			{
				r.color = new Color(1, 1, 1, alpha);
			}
			yield return null;
		}

		GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull().Release(gameObject);
	}
}
