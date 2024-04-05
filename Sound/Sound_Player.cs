using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using Utils;

enum GroundType
{
	None = -1,
	Carpet,
	Grass,
	Wood,
}

public class Sound_Player : MonoBehaviour
{
	private Rigidbody mRigidbody;


	[SerializeField]
	private Transform mModelTransform;

	[ShowInInspector]
	private float mSpeed = 10.0f;

	private Vector3 mDir = new Vector3();

	[SerializeField]
	private Transform mFoot;

	[SerializeField]
	private float mFootTick;

	private GroundType mCurrentFootGroundType;

	private bool mIsWalk = false;

	private Vector2 mPlayerScreenPos;
	private Vector2 mMouseScreenPos;
	private Vector3 mPlayerToMouseDir;

	private bool mIsLookRight = false;

	[Title("Weapon")]
	[SerializeField]
	private Transform mWeaponPivot;
	[SerializeField]
	private Transform mWeapon;

	private BaseFmodSoundService mFmodSoundService;

	private void Awake()
	{
		mRigidbody = GetComponent<Rigidbody>();
		mFmodSoundService = GlobalServiceLocator.SoundService.GetServiceOrNull();
	}

	private void Start()
	{
		StartCoroutine(footSound());
	}

	private void Update()
	{
		mMouseScreenPos = Input.mousePosition;
		mPlayerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
		mPlayerToMouseDir = (mMouseScreenPos - mPlayerScreenPos).normalized;

		if (Input.GetMouseButtonDown(0))
		{
			shot();
		}

		look();
		gunRoation();
	}

	private void FixedUpdate()
	{
		movement();
	}

	private void movement()
	{
		mDir.x = Input.GetAxisRaw("Horizontal");
		mDir.z = Input.GetAxisRaw("Vertical");
		mDir.Normalize();

		mIsWalk = mDir.sqrMagnitude > 0.0f;

		mRigidbody.MovePosition(gameObject.transform.position + mDir * mSpeed * Time.deltaTime);
	}

	private void look()
	{
		Vector3 modelScale = Vector3.one;
		mIsLookRight = false;
		if (mPlayerToMouseDir.x > 0.0f)
		{
			modelScale = new Vector3(-1.0f, 1.0f, 1.0f);
			mIsLookRight = true;
		}


		mModelTransform.localScale = modelScale;
	}

	private void OnCollisionEnter(Collision collision)
	{
		mCurrentFootGroundType = GroundType.None;

		if (collision.collider.tag.Equals("Carpet"))
			mCurrentFootGroundType = GroundType.Carpet;
		else if (collision.collider.tag.Equals("Grass"))
			mCurrentFootGroundType = GroundType.Grass;
		else if (collision.collider.tag.Equals("Wood"))
			mCurrentFootGroundType = GroundType.Wood;

		Debug.Log(mCurrentFootGroundType);
	}

	private IEnumerator footSound()
	{
		while(true)
		{
			while (mIsWalk)
			{
				int groundType = (int)mCurrentFootGroundType;

				SoundParameter parameter = new SoundParameter("Surface", groundType);
				mFmodSoundService?.Play(SoundType.SFX_FootStep, parameter);
				yield return new WaitForSeconds(mFootTick);
			}
			yield return null;
		}
	}

	private void shot()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//if(Physics.Raycast(ray, out hit))
		//{
		//	if(hit.transform.gameObject.TryGetComponent<Boom>(out var boom))
		//	{
		//		boom.Action();
		//	}
		//}

		mFmodSoundService?.Play(SoundType.SFX_Pistol,transform.position);
	}

	private void gunRoation()
	{
		Vector2 pivotToScreen = Camera.main.WorldToScreenPoint(mWeaponPivot.position);
		Vector2 pivotToMouse = mMouseScreenPos - pivotToScreen;

		var angle = Mathf.Atan2(pivotToMouse.y, pivotToMouse.x) * Mathf.Rad2Deg;

		var finalAngle = angle - 180.0f;

		mWeaponPivot.eulerAngles = new Vector3(0.0f, 0.0f, finalAngle);

		var absFinalAngle = Mathf.Abs(finalAngle);

		Vector3 gunFlip = Vector3.one;

		if(mIsLookRight)
		{
			gunFlip.y = -1.0f;
		}

		mWeapon.localScale = gunFlip;
	}

}
