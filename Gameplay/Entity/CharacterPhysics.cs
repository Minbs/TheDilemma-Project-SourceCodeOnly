using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Gameplay;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterPhysics : MonoBehaviour
{
	private Rigidbody2D mRigidbody;
	//private Collider2D mCollider;

	#region Movement

	[TabGroup("Move"), ShowInInspector]
	public float MoveSpeed { get; private set; } = 5;
	[TabGroup("Dash"), ShowInInspector]
	public float DashDistance { get; private set; } = 5;
	[TabGroup("Dash"), ShowInInspector]
	public float DashDuration { get; private set; } = 1;
	private bool mIsDash;

	#endregion

	private void Awake()
	{
		mRigidbody = GetComponent<Rigidbody2D>();
		//mCollider = GetComponent<Collider2D>();
	}

	public void Setup(float moveSpeed)
	{
		MoveSpeed = moveSpeed;
	}

	public void OnInitialized()
	{
		mIsDash = false;
	}

	public void Move(Vector2 direction)
	{
		if (mIsDash)
		{
			return;
		}

		direction.Normalize();
		direction = new Vector2(direction.x, direction.y * GlobalGameplayData.WorldBasisRatio);
		mRigidbody.velocity = direction * MoveSpeed;
	}

	public void Dash(Vector3 direction)
	{
		StartCoroutine(dashRoutine(direction));

		IEnumerator dashRoutine(Vector3 direction, bool isNow = true)
		{
			if (mIsDash || !isNow)
			{
				yield break;
			}

			float proceed = 0;
			Vector3 start = transform.position;
			Vector3 target = transform.position + direction * DashDistance;
			mIsDash = true;

			while (proceed <= 1)
			{
				proceed += Time.fixedDeltaTime / DashDuration;
				mRigidbody.MovePosition(Vector2.Lerp(start, target, proceed));

				yield return null;
			}

			mRigidbody.MovePosition(target);
			mIsDash = false;
		}
	}
}
