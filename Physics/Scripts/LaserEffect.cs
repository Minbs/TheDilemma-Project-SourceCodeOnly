using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{
    private Rigidbody2D mRigidbody;
    private Vector2 mDirection;
    private float mMaxDistance;
    private bool mIsPenetrate;

	private float mDistance = 0;
	private bool mIsCheckCollider = false;

	private readonly string EntityHitLayerName = "EntityHitbox";
	private readonly string WallLayerName = "WallCollider"; // 벽 콜라이더 레이어

	#region single
	private RaycastHit2D mSingleHit;
	#endregion

	#region penetrate
	private RaycastHit2D mWallHit; // 닿은 벽의 콜라이더
    private List<RaycastHit2D> mEntityHits = new(); // 레이에 닿은 Entity의 리스트
	#endregion

	[field : SerializeField] public GameObject LaserStart { get; set; }
    [field: SerializeField] public GameObject LaserMiddle { get; set; }
    [field: SerializeField] public GameObject LaserEnd { get; set; }

    public void OnInitialized(Vector3 position, Vector2 direction, float MaxDistance, bool isPenetrate = false)
    {
        transform.position = position;
        transform.right = direction;
        mDirection = direction;
		mMaxDistance = MaxDistance;
        mIsPenetrate = isPenetrate;
	}

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody2D>();
		LaserStart.transform.localPosition = Vector2.zero;
		LaserMiddle.transform.localPosition = Vector2.zero;
		LaserEnd.transform.localPosition = Vector2.zero;
		Destroy(gameObject, 1);
    }

	/// <summary> 레이저 상태에 따라 이펙트를 내보내는 함수 /// </summary>
	private void DrawLaser()
	{
		bool isHit = mSingleHit.collider != null || mWallHit.collider != null;

		LaserEnd.SetActive(isHit);

		LaserStart.SetActive(true);
		LaserMiddle.SetActive(true);

		//GameObject middle = Instantiate(LaserMiddle, gameObject.transform);
		//middle.transform.localScale = 


		LaserMiddle.transform.localScale = new Vector3(mDistance, LaserMiddle.transform.localScale.y, LaserMiddle.transform.localScale.z);
		LaserMiddle.transform.localPosition = new Vector2((mDistance / 2f), 0f);
		LaserEnd.transform.localPosition = new Vector2(mDistance, 0f);
	}	

    private void FixedUpdate()
    {
		if(!mIsCheckCollider) // ray를 계산하기 전에 실행
		{
			if (!mIsPenetrate)
			{
				mSingleHit = GetSingleRaycastHit();
				if(mSingleHit.collider != null && mSingleHit.transform.gameObject.layer.Equals(LayerMask.GetMask(EntityHitLayerName)))
				{
					mEntityHits.Add(mSingleHit);
				}
			}
			else
			{
				mWallHit = GetWallRaycastHit();
				mEntityHits = GetPenetrateRaycastHit().ToList();
			}

			mIsCheckCollider = true;
		}
		else
		{
			DrawLaser();
		}
	}

    private RaycastHit2D GetWallRaycastHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mDirection, mMaxDistance, LayerMask.GetMask(WallLayerName));

		if (hit.collider != null)
		{
			mDistance = Vector2.Distance(hit.point, transform.position);
		}
		else
		{
			mDistance = mMaxDistance;
		}

		return hit;
    }

    private RaycastHit2D[] GetPenetrateRaycastHit()
    {
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, mDirection, mDistance, LayerMask.GetMask(EntityHitLayerName));
		foreach (var entityHit in hits)
		{
			Debug.Log(entityHit.collider.gameObject.name);
		}
		return hits;

	}

    private RaycastHit2D GetSingleRaycastHit()
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, mDirection, mMaxDistance, LayerMask.GetMask(WallLayerName, EntityHitLayerName));

		if(hit.collider != null)
		{
			mDistance = Vector2.Distance(hit.point, transform.position);
		}
		else
		{
			mDistance = mMaxDistance;
		}

		return hit;
    }
}
