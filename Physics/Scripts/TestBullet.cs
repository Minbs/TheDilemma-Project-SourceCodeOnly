using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
	private SpriteRenderer mSpriteRenderer;
	private Rigidbody2D mRigidbody;
	private Vector3 mStartPosition;
	private Vector2 mDirection;
	private float mMoveSpeed;

	public void Initialize(Sprite bulletSprite, Vector2 direction, float moveSpeed)
	{
		if (mSpriteRenderer == null)
		{
			mSpriteRenderer = GetComponent<SpriteRenderer>();
			mRigidbody = GetComponent<Rigidbody2D>();
		}
		mSpriteRenderer.sprite = bulletSprite;
		mStartPosition = transform.position;
		Debug.Log(direction);
		mDirection = direction;
		mMoveSpeed = moveSpeed;

		Move();
	}

	// Start is called before the first frame update
	void Start()
	{
		if (mSpriteRenderer == null)
		{
			mSpriteRenderer = GetComponent<SpriteRenderer>();
		}

		if (mRigidbody == null)
		{
			mRigidbody = GetComponent<Rigidbody2D>();
		}

	}

	public void Move(bool isNow = true)
	{
		if (isNow)
			mRigidbody.velocity = mDirection * mMoveSpeed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var curLayer = GlobalLayer.GetMask(collision);
		if (!GlobalLayer.IsMatch(curLayer, GlobalLayer.LAYER_RAYCAST_HITBOX))
		{
			return;
		}

		Debug.Log(collision.gameObject.layer.ToString());
		Debug.Log(collision.gameObject.name);

		GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull().Release(gameObject);
	}
}
