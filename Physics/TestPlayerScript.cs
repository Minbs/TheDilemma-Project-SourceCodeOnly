using Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Utils;

public class TestPlayerScript : MonoBehaviour
{
	public TestBulletPool TestPool;
	private CharacterPhysics mCharacterPhysics;
	public Transform AimTransform;
	public Transform ShootPivot;
	public Sprite BulletSprite;

	public bool LookUp = false;
	public Vector2 AimDirection = Vector2.zero;
	public Equipment_Gun primaryGun;
	public Equipment_Gun secondaryGun;


	[field: SerializeField] public SerializableDictionary<EquipmentType, GameObject> EquipmentGunTable { get; set; } = new();
	//public Tilemap tileMap;

	// Start is called before the first frame update
	void Start()
	{
		mCharacterPhysics = GetComponent<CharacterPhysics>();
	}

	// Update is called once per frame
	void Update()
	{
		Vector2 move = Vector2.zero;


		if (Input.GetKey(KeyCode.W))
		{
			move.y += 1;
		}
		if (Input.GetKey(KeyCode.A))
		{
			move.x -= 1;
		}
		if (Input.GetKey(KeyCode.D))
		{
			move.x += 1;
		}
		if (Input.GetKey(KeyCode.S))
		{
			move.y -= 1;
		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			primaryGun.ChangeSecondaryGun();
			secondaryGun.ChangePrimaryGun();

			Equipment_Gun temp = primaryGun;
			primaryGun = secondaryGun;
			secondaryGun = temp;

			AimTransform = primaryGun.AimTransform;
			ShootPivot = primaryGun.MuzzleTransform;
		}

		if(Input.GetKeyDown(KeyCode.K))
		{
			if(!EquipmentGunTable.TryGetValue(EquipmentType.HeavyPistol, out GameObject gun))
			{
				Debug.LogError("√— æ¯¿Ω");
			}
			else
			{
				Equipment_Gun equipment_Gun = Instantiate(gun, transform).GetComponent<Equipment_Gun>();
				if (equipment_Gun != null)
				{
					Destroy(primaryGun.gameObject);
					primaryGun = equipment_Gun;
					primaryGun.ChangePrimaryGun();
					AimTransform = primaryGun.AimTransform;
					ShootPivot = primaryGun.MuzzleTransform;
				}
				//primaryGun.gameObject = gun;

			}
		}

		if(Input.GetKeyDown(KeyCode.V))
		{
			StartCoroutine(GetHit());
		}

		if(Input.GetKey(KeyCode.R))
		{
			Reload();
		}

		move = move.normalized;

		if (Input.GetKey(KeyCode.Space))
		{
			mCharacterPhysics.Dash(move);
		}
		else
		{
			mCharacterPhysics.Move(move);
		}


		Vector3 direction = MouseExtension.GetMouseDirectionBy(transform);
		

		if (Input.GetMouseButtonDown(0))
		{
			shoot(direction);
		}
		if (Input.GetMouseButtonDown(1))
		{
			hitScan(direction);
		}



		if (!mIsReloading)
		{
			AimDirection = Vector3.Lerp( AimDirection, direction,Time.deltaTime * 10);
			flip(AimDirection);
			aim(AimDirection);
		}
		else
		{
			PlayReloadAnimation();
		}

	}

	//private void GetTileOnGround()
	//{
	//	Vector3Int pos = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
	//	var tile = tileMap.GetTile(pos);
	//}

	private void FixedUpdate()
	{
		int baseSortOrder = Global.RoundByDepthOrderOffset(transform);
		var childrenRenderer = transform.GetComponentsInChildren<Renderer>();
		foreach (var renderer in childrenRenderer)
		{
			renderer.sortingOrder = baseSortOrder;
		}

		secondaryGun.Weapon_Sprite.sortingOrder = secondaryGun.Weapon_Sprite.sortingOrder - 1;
	}

	private void flip(Vector3 direction)
	{
		Vector3 tempScale = mCharacterPhysics.transform.localScale;
		tempScale.x = Mathf.Abs(tempScale.x);

		// øﬁ¬ ¿ª πŸ∂Û∫Ω
		if (Vector2.Dot(direction, Vector2.right) < 0)
		{
			tempScale.x *= -1;
		}

		mCharacterPhysics.transform.localScale = tempScale;
	}

	private void shoot(Vector3 direction)
	{
		var bullet = TestPool.GetObejct<TestBullet>(ShootPivot.transform.position + direction);
		bullet.Initialize(BulletSprite, direction, 20);
	}

	private bool mIsReloading = false;
	private float mReloadTimer = 0;
	private float mReloadDuration = 1;
	private Vector3 startRotation = new();
	private Vector3 endRotation = new(0, 0, -30);

	[field: SerializeField] private AnimationCurve mReloadAnimationCurve;
	private void Reload()
	{
		if(!mIsReloading)
		{
			startRotation = AimTransform.localEulerAngles;
			mIsReloading = true;
		}
	}

	private void PlayReloadAnimation()
	{
		if(mReloadTimer < mReloadDuration)
		{
			mReloadTimer += Time.deltaTime;
			float value = mReloadAnimationCurve.Evaluate(mReloadTimer);
			Vector3 aimDirection = Vector2.Dot(AimDirection, Vector2.right) > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 0, 180);
			AimTransform.localRotation = Quaternion.Lerp(startRotation.ToQuaternion(), (endRotation + aimDirection).ToQuaternion() , value);
		}
		else
		{
			AimDirection = AimTransform.right;
			mIsReloading = false;
			mReloadTimer = 0;
		}
	}

	private void hitScan(Vector3 direction)
	{
		var instanceLaser = TestPool.GetObejct<Effect_HitscanLaser>(ShootPivot.transform.position);
		Ulog.LogWarning(this, $"∑π¿Ã¿˙ ¿Ã∆Â∆Æ ¡¶∞≈µ ");
		//instanceLaser.Initialize(ShootPivot.position, direction, 10);
	}



	


	/// <summary>  √— ∏∂øÏΩ∫ πÊ«‚ πŸ∂Û∫∏±‚ /// </summary>
	private void aim(Vector3 direction)
	{
		// Set look direction

		float lookFactor = Vector2.Dot(direction, Vector2.up);
		if (LookUp && lookFactor < 0)
		{
			LookUp = false;
		}
		else if (lookFactor > 0.4)
		{
			LookUp = true;
		}

		// Set weapon aim direction
		float aimFactor = Vector2.Dot(direction, Vector2.right);
		float scaleFactor = (aimFactor < 0) ? -1 : 1;
		AimTransform.right = direction;
		AimTransform.localScale = new Vector3(scaleFactor, scaleFactor);
	}
	

	public float hitDuration = 2;
	private float hitTimer = 0;

	public float hitMaxDuration = 2.0f;
	private float hitMaxTimer = 0;
	public SpriteRenderer characterRenderer;

	public IEnumerator GetHit()
	{
		var mat = characterRenderer.material;
		hitTimer = 0;
		hitMaxTimer = 0;

		while (hitTimer < hitDuration)
		{
			hitTimer += Time.deltaTime;
			var value = Mathf.Lerp(0, 1, hitTimer / hitDuration);
			mat.SetFloat("_HitColorProceed", value);
			yield return null;
		}

		yield return null;

		while (hitMaxTimer < hitMaxDuration)
		{
			hitMaxTimer += Time.deltaTime;
			mat.SetFloat("_HitColorProceed", 1);
			yield return null;
		}

		mat.SetFloat("_HitColorProceed", 0);
		yield return null;
	}
}
