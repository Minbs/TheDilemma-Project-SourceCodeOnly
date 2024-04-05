using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Utils.ViewModel;
using Gameplay;
using UnityEngine.EventSystems;
using System;
using KaNet.Compensation;
using System.IO.Pipes;

[RequireComponent(typeof(Proxy_EntityPlayer))]
public class Entity_PlayerController : EntityBase
{
	public override void OnValidate()
	{
		base.OnValidate();
		PlayerProxy = GetComponent<Proxy_EntityPlayer>();
	}

	/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
	public override NetObjectType Type => NetObjectType.Entity_Player;

	[field: SerializeField] public Proxy_EntityPlayer PlayerProxy { get; private set; }
	[SerializeField] private CharacterPhysics mCharacterPhysics;

	//private GenericViewModel<Transform> Pivot_Camera = new(nameof(Pivot_Camera));
	public Transform CameraTarget => Pivot_Aim.Model;
	private GenericViewModel<Transform> Pivot_Muzzle = new(nameof(Pivot_Muzzle));
	public Transform MuzzlePivot => Pivot_Muzzle.Model;
	private GenericViewModel<Transform> Pivot_Aim = new(nameof(Pivot_Aim));
	public Transform AimPivot => Pivot_Aim.Model;

	private void Awake()
	{
		//this.Pivot_Camera.Initialize(this);
		this.Pivot_Muzzle.Initialize(this);
		this.Pivot_Aim.Initialize(this);
	}

	// Inputs
	[SyncVar(SyncType.UnreliableFixed, SyncAuthority.OwnerBroadcast)]
	private readonly SyncFieldByOrder<NetVector3> mPositionByClient = new();
	[SyncVar(SyncType.UnreliableFixed, SyncAuthority.OwnerBroadcast)]
	private readonly SyncFieldByOrder<NetVector2> mViewDirectionByClient = new();

	[SyncVar(Authority : SyncAuthority.OwnerBroadcast)]
	private readonly SyncField<NetUInt8<AnimationType>> mAnimationTypeByClient = new();

	[SyncVar]
	public readonly SyncField<NetUInt8<CharacterType>> CharacterType = new();

	[SyncVar]
	public readonly SyncField<EntityBasicData> BasicData = new();

	// Equipment
	[SyncVar]
	public readonly SyncField<EquipmentData> Equipment_Primary = new();

	[SyncVar]
	public readonly SyncField<EquipmentData> Equipment_Secondary = new();

	[SyncVar]
	public readonly SyncField<EquipmentData> Equipment_Auxillary = new();

	[SyncVar]
	public readonly SyncField<NetUInt8<BaseEquipmentType>> CurrentEquipment = new();

	private BidirectionalMap<BaseEquipmentType, EquipmentState> mEquipmentTable = new();

	public EquipmentState CurrentEquipmentState { get; protected set; }

	#region Getter

	public bool TryGetPrimaryState(out EquipmentState state)
	{
		return mEquipmentTable.TryGetValue(BaseEquipmentType.Primary, out state);
	}

	public bool TryGetSecondaryState(out EquipmentState state)
	{
		return mEquipmentTable.TryGetValue(BaseEquipmentType.Secondary, out state);
	}

	public bool TryGetAuxilliaryState(out EquipmentState state)
	{
		return mEquipmentTable.TryGetValue(BaseEquipmentType.Auxilliary, out state);
	}

	#endregion

	public override void Common_OnStart()
	{
		this.mCharacterPhysics.OnInitialized();
		this.PlayerProxy.Initialize();

		mPositionByClient.ResetTimestamp();
		mPositionByClient.ResetDeserializeEvent();

		mViewDirectionByClient.ResetTimestamp();
		mViewDirectionByClient.ResetDeserializeEvent();

		// 자기 자신이 조종할 수 없는 Player Control 객체는 밀리면 안된다.
		EntityRigid.bodyType = IsOwner ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

		// Binding equipment data
		Equipment_Primary.OnDeserialized += applyEquipmentData;
		Equipment_Secondary.OnDeserialized += applyEquipmentData;
		Equipment_Auxillary.OnDeserialized += applyEquipmentData;

		applyEquipmentData(Equipment_Primary.Data);
		applyEquipmentData(Equipment_Secondary.Data);
		applyEquipmentData(Equipment_Auxillary.Data);

		this.ProcessEquipmentSwapInput(1);

		// Initialize Proxy
		initializeProxy();
	}

	public override void Common_OnDestroy()
	{
		// Unbinding equipment data
		Equipment_Primary.OnDeserialized -= applyEquipmentData;
		Equipment_Secondary.OnDeserialized -= applyEquipmentData;
		Equipment_Auxillary.OnDeserialized -= applyEquipmentData;
	}

	public void applyEquipmentData(EquipmentData equipmentData)
	{
		var baseType = equipmentData.Equipment.GetEnum().GetBaseType();

		EquipmentState data;

		if (!mEquipmentTable.TryGetValue(baseType, out data))
		{
			var equipState = new EquipmentState();
			mEquipmentTable.TryAdd(baseType, equipState);
			data = equipState;
		}

		data.Initialize(equipmentData);
	}

	public override void Server_OnStart()
	{
		base.Server_OnStart();
		mPositionByClient.Data = transform.position;
	}

	public override void Client_OnStart()
	{
		base.Client_OnStart();

		transform.position = mPositionByClient.Data;
		PlayerProxy.LookAt(mViewDirectionByClient.Data);
	}

	public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		// For debug
		if (IsServerSide)
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				mHP.Data = mMaxHP.Data;
			}

			if (Input.GetKeyDown(KeyCode.O))
			{
				mHP.Data -= 20;
				if (mHP.Data <= 0)
				{
					Kill();
				}
			}
		}

		if (CurrentEquipmentState == null)
		{
			if (mEquipmentTable.TryGetValue(CurrentEquipment.Data.GetEnum(), out var state))
			{
				CurrentEquipmentState = state;
			}
		}

		CurrentEquipmentState?.Tick(in deltaTimeInfo);

		PlayerProxy.OnUpdate(deltaTimeInfo);

		if (IsOwner)
		{
			mAnimationTypeByClient.Data = PlayerProxy.CurrentAnimation;
			mPositionByClient.Data = transform.position;
		}
		else
		{
			transform.position = Vector3.Lerp
			(
				transform.position, 
				mPositionByClient.Data, 
				KaNetGlobal.NETWORK_INTERPOLATION_VALUE * deltaTimeInfo.ScaledDeltaTime
			);
		}

		var look = Vector2.Lerp
		(
			PlayerProxy.LookDirection,
			mViewDirectionByClient.Data,
			KaNetGlobal.NETWORK_INTERPOLATION_VALUE * deltaTimeInfo.ScaledDeltaTime
		);

		PlayerProxy.LookAt(look);
		PlayerProxy.PlayAnimation(mAnimationTypeByClient.Data.GetEnum());
	}

	public void Server_InitializePlayer(PlayerCharacterData data, FactionType factionType)
	{
		mFaction.Data = factionType;
		mMaxHP.Data = data.MaxHp;
		mHP.Data = data.MaxHp;
		mMoveSpeed.Data = data.MoveSpeed;
		CharacterType.Data = data.CharacterType;

		Equipment_Primary.Data = data.Primary;
		Equipment_Secondary.Data = data.Secondary;
		Equipment_Auxillary.Data = data.Auxillary;

		applyEquipmentData(data.Primary);
		applyEquipmentData(data.Secondary);
		applyEquipmentData(data.Auxillary);


		// TODO : Initialize proxy

		initializeProxy();
	}

	private void initializeProxy()
	{
		PlayerProxy.SetCharacterType(CharacterType.Data.GetEnum());
	}

	public override void Server_InitializeByEntityService(EntityData entityData, FactionType faction)
	{
		base.Server_InitializeByEntityService(entityData, faction);
		mCharacterPhysics.Setup(MoveSpeed);
	}

	[RpcCall]
	public readonly RpcCaller<NetVector2> RPC_Server_Teleport = new();
	private void Server_Teleport(NetVector2 position)
	{
		if (IsOwner)
		{
			mPositionByClient.Data = position.Value.ToVector3();
			transform.position = position;
		}
	}

	#region Player Input

	private NetVector2 getMouseDirectionByAim()
	{
		return MouseExtension.GetMouseDirectionBy(AimPivot);
	}

	public void ProcessDesh(Vector2 dashInput)
	{
		this.mCharacterPhysics.Dash(dashInput);
	}

	public void ProcessEquipmentSwapInput(int index)
	{
		if (index < 1 || index > 3)
		{
			return;
		}

		var baseType = (BaseEquipmentType)index;

		CurrentEquipment.Data = baseType;
		if (mEquipmentTable.TryGetValue(baseType, out var equipmentState))
		{
			CurrentEquipmentState = equipmentState;
		}
	}

	public void ProcessMovementInput(NetVector2 moveInput)
	{
		var mouseDirection = getMouseDirectionByAim();

		mCharacterPhysics.Move(moveInput);
		mViewDirectionByClient.Data = mouseDirection;

		if (moveInput != Vector2.zero)
		{
			PlayerProxy.Move(Time.deltaTime);
		}
	}

	public void ProcessHealSelf()
	{

	}

	public void ProcessInteract()
	{

	}

	public void ProcessReload()
	{
		var baseType = CurrentEquipment.Data.GetEnum();

		if (mEquipmentTable.TryGetValue(baseType, out var equipmentState))
		{
			equipmentState.TryReload();
		}
	}

	public void ProcessMousePressed(bool isLeft)
	{
		if (isLeft)
		{
			useEquipment(false);
		}
		else
		{
			ProcessAltFireInput();
		}
	}

	public void ProcessMousePressing(bool isLeft)
	{
		if (isLeft)
		{
			useEquipment(true);
		}
		else
		{
			ProcessAltFireInput();
		}
	}

	private void useEquipment(bool isPressing)
	{
		NetVector2 fireDirection = ((MuzzlePivot.position - AimPivot.position).normalized).ToVector2();

		var baseType = CurrentEquipment.Data.GetEnum();

		if (mEquipmentTable.TryGetValue(baseType, out var equipmentState))
		{
			if (!((equipmentState.FireType == FireModeType.Auto && isPressing) ||
				(equipmentState.FireType == FireModeType.SemiAuto && !isPressing)))
			{
				return;
			}

			if (equipmentState.TryUse() == EquipmentUseResult.Success)
			{
				WeaponInfo weaponInfo = equipmentState.InitialData.WeaponInfo;

				// Calculate accuracy
				var acc = equipmentState.Accuracy;
				Vector2 moa = new Vector2
				(
					UnityEngine.Random.Range(-acc, acc), 
					UnityEngine.Random.Range(-acc, acc)
				);

				fireDirection = (fireDirection.Value * GlobalGameplayData.MOA_DISTANCE + moa).normalized;
				
				// Set point if it's instant point
				if (weaponInfo.HitscanType.GetEnum() == HitscanType.Hitscan_Instant_Point)
				{
					fireDirection = MouseExtension.GetWorldMousePosition();
				}

				mGameplayManager.HitscanHandler.CreateHitscanAsOwner
				(
					this, weaponInfo, MuzzlePivot.position, fireDirection
				);

				// Shake camera
				if (IsOwner)
				{
					var rec = equipmentState.InitialData.Recoil;
					Vector2 recoil = new Vector2(UnityEngine.Random.Range(-rec, rec), UnityEngine.Random.Range(-rec, rec));
					mGameplayManager.PlayerCamera.AddImpulse(recoil);
				}
			}
		}
	}

#if UNITY_EDITOR
	/// <summary>부가 정보를 출력합니다.</summary>
	public void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		NetVector2 fireDirection = ((MuzzlePivot.position - AimPivot.position).normalized).ToVector2();

		var baseType = CurrentEquipment.Data.GetEnum();
		if (mEquipmentTable.TryGetValue(baseType, out var equipmentState))
		{
			var acc = equipmentState.Accuracy;

			//Debug.DrawRay(AimPivot, )
			Vector2 lb = new Vector2(-acc, -acc);
			Vector2 lt = new Vector2(-acc, acc);
			Vector2 rb = new Vector2(acc, -acc);
			Vector2 rt = new Vector2(acc, acc);

			var rlb = fireDirection.Value * GlobalGameplayData.MOA_DISTANCE + lb;
			var rlt = fireDirection.Value * GlobalGameplayData.MOA_DISTANCE + lt;
			var rrb = fireDirection.Value * GlobalGameplayData.MOA_DISTANCE + rb;
			var rrt = fireDirection.Value * GlobalGameplayData.MOA_DISTANCE + rt;

			Debug.DrawRay(AimPivot.position, rlb, Color.red);
			Debug.DrawRay(AimPivot.position, rlt, Color.red);
			Debug.DrawRay(AimPivot.position, rrb, Color.red);
			Debug.DrawRay(AimPivot.position, rrt, Color.red);
		}
	}
#endif

	public void ProcessAltFireInput()
	{

	}

	#endregion
}
