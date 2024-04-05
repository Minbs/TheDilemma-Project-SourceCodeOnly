using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UIElements;

public class Effect_HitscanLaser : Effect_HitscanBase
{
	[Title("Effect")]
	//[field : SerializeField] public Sprite BulletMarkSprite { get; set; }
	[field : SerializeField] public GameObject MiddleMarkInstance { get; private set; }
	[field : SerializeField][Range(0, 2.0F)] public float MarkRemainingDelay = 0.3F;

	private List<GameObject> mBulletMarkObjects = new();
	private List<SpriteRenderer> mSprites = new();

	public override void InitializeHitscan(Vector3 start, Vector3 end)
	{
		var distance = (end - start);

		transform.right = distance.normalized;
		transform.position = transform.position.GetOnlyXY();

		int objCount = (int)(Mathf.Abs(distance.y) / Effect_HitscanBase.ONE_UNIT_DIVISION);

		if (objCount == 0)
		{
			objCount = 1;
		}

		for (int i = 0; i < objCount; i++)
		{
			Vector2 pos = Vector2.Lerp(start, end, (float)i / objCount);
			Vector2 nextPos = Vector2.Lerp(start, end, (float)(i + 1) / objCount);

			float length = (nextPos - pos).magnitude;

			var go = createObject(MiddleMarkInstance, pos.ToVector3(), Quaternion.identity);
			go.transform.parent = transform;
			go.transform.right = transform.right;
			go.transform.localScale = new Vector3
			(
				length,
				go.transform.localScale.y,
				go.transform.localScale.z
			);

			var renderer = go.GetComponent<SpriteRenderer>();
			renderer.sortingOrder = Global.RoundByHitscanDepthOrder(go.transform);
			mSprites.Add(renderer);
			mBulletMarkObjects.Add(go);
		}
		
		StartCoroutine(runAnimation());
	}

	private IEnumerator runAnimation()
	{
		for (float alpha = MarkRemainingDelay; alpha >= 0; alpha -= Time.deltaTime)
		{
			foreach (var s in mSprites)
			{
				if (s != null)
				{
					if (s.isVisible)
					{
						var newColor = s.color;
						newColor.a = alpha / MarkRemainingDelay;
						s.color = newColor;
					}
				}
			}

			yield return null;
		}

		foreach (var go in mBulletMarkObjects)
		{
			this.releaseObject(go);
		}

		mSprites.Clear();
		mBulletMarkObjects.Clear();

		GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull().Release(gameObject);
	}
}
