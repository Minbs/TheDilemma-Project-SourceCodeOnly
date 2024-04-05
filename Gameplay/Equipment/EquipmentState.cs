using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;

namespace Gameplay
{
	/// <summary>장비의 상태입니다.</summary>
	public enum EquipmentActionState
	{
		None = 0,
		/// <summary>다 사용한 상태입니다.</summary>
		Empty,
		/// <summary>사용(발사) 가능한 상태입니다.</summary>
		ReadyToUse,
		/// <summary>쿨타임 상태입니다.</summary>
		UseDelaying,
		/// <summary>재장전 중입니다.</summary>
		Reloading,
	}

	public enum EquipmentUseResult
	{
		None = 0,
		Success,
		/// <summary>재장전할 탄약이 없습니다.</summary>
		NoAmmoToReload,
		/// <summary>탄창에 탄약이 없습니다.</summary>
		NoAmmoInMagazine,
		/// <summary>이미 탄약이 가득 찬 상태입니다.</summary>
		MagazineIsFull,
		Reloading,
		Cooltime,
	}

	public class EquipmentState
	{
		public event Action OnReloaded;
		public event Action OnMagazineEmpty;

		public EquipmentData InitialData { get; private set; }

		// Ammunition
		/// <summary>탄창 용량입니다.</summary>
		public int MagazineCapacity { get; private set; }
		/// <summary>현재 탄창의 용량입니다.</summary>
		public int Magazine { get; private set; }
		/// <summary>소유 가능한 최대 탄약입니다.</summary>
		public int MaxAmmoOwned { get; private set; }
		/// <summary>소유한 탄약입니다.</summary>
		public int AmmoOwned { get; private set; }

		// UseDelay
		public EquipmentActionState State { get; private set; }
		public float InitialUseDelay { get; private set; }
		public float UseDelay { get; private set; }
		/// <summary>0 ~ 1 사이로 발사 사이 진행도를 표시합니다. 시작시 1이고 완료되면 0입니다.</summary>
		public float UseDelayProgress => UseDelay / InitialUseDelay;
		public float InitialReloadDelay { get; private set; }
		public float ReloadDelay { get; private set; }
		/// <summary>0 ~ 1 사이로 재장전 진행도를 표시합니다. 시작시 1이고 완료되면 0입니다.</summary>
		public float ReloadProgress => ReloadDelay / InitialReloadDelay;

		// Accuracy
		public float MinAccuracy { get; private set; }
		public float MaxAccuracy { get; private set; }
		public float AccuracyRecovery { get; private set; }
		public float AccuracyIncrease { get; private set; }
		public float Accuracy { get; private set; }

		public FireModeType FireType => InitialData.FireMode.GetEnum();

		public bool HasAmmo => AmmoOwned > 0;

		public void Initialize(in EquipmentData initialData)
		{
			OnReloaded = null;
			OnMagazineEmpty = null;

			InitialData = initialData;

			// Ammunition
			MagazineCapacity = initialData.MagazineCapacity;
			Magazine = MagazineCapacity;

			MaxAmmoOwned = initialData.MaxAmmo;
			AmmoOwned = MaxAmmoOwned;

			// UseDelay
			State = EquipmentActionState.ReadyToUse;
			InitialUseDelay = initialData.UseDelay;
			UseDelay = 0;

			InitialReloadDelay = initialData.ReloadDelay;
			ReloadDelay = 0;

			// Accuracy
			MinAccuracy = initialData.MinAccuracy;
			MaxAccuracy = initialData.MaxAccuracy;
			AccuracyRecovery = initialData.AccuracyRecovery;
			AccuracyIncrease = initialData.AccuracyIncrease;
			Accuracy = MinAccuracy;
		}

		public void Tick(in DeltaTimeInfo deltaTimeInfo)
		{
			float deltaTime = deltaTimeInfo.ScaledDeltaTime;

			if (Accuracy < MinAccuracy)
			{
				Accuracy = MinAccuracy;
			}

			Accuracy = Mathf.Lerp(Accuracy, MinAccuracy, AccuracyRecovery * deltaTime);

			switch (State)
			{
				case EquipmentActionState.ReadyToUse:
					if (ReloadDelay > 0)
					{
						State = EquipmentActionState.Reloading;
						break;
					}
					if (UseDelay > 0)
					{
						State = EquipmentActionState.UseDelaying;
						break;
					}
					break;

				case EquipmentActionState.UseDelaying:
					if (UseDelay > 0)
					{
						UseDelay -= deltaTime;
					}
					else
					{
						if (Magazine > 0)
						{
							State = EquipmentActionState.ReadyToUse;
						}
						else
						{
							State = EquipmentActionState.Empty;
							OnMagazineEmpty?.Invoke();
						}
					}
					break;

				case EquipmentActionState.Reloading:
					if (ReloadDelay > 0)
					{
						ReloadDelay -= deltaTime;
					}
					else
					{
						if (!HasAmmo)
						{
							State = EquipmentActionState.Empty;
							break;
						}

						int fillAmount = MagazineCapacity - Magazine;

						if (AmmoOwned < fillAmount)
						{
							fillAmount = AmmoOwned;
						}

						AmmoOwned -= fillAmount;
						Magazine += fillAmount;

						State = EquipmentActionState.ReadyToUse;
						OnReloaded?.Invoke();
					}
					break;
			}
		}

		public EquipmentUseResult TryReload()
		{
			if (Magazine >= MagazineCapacity)
			{
				return EquipmentUseResult.MagazineIsFull;
			}

			if (State == EquipmentActionState.Reloading)
			{
				return EquipmentUseResult.Reloading;
			}

			if (AmmoOwned <= 0)
			{
				return EquipmentUseResult.NoAmmoToReload;
			}

			ReloadDelay = InitialReloadDelay;
			State = EquipmentActionState.Reloading;

			return EquipmentUseResult.Success;
		}

		public EquipmentUseResult TryUse()
		{
			if (State == EquipmentActionState.Reloading)
			{
				return EquipmentUseResult.Reloading;
			}
			else if (State == EquipmentActionState.UseDelaying)
			{
				return EquipmentUseResult.Cooltime;
			}
			else if (State == EquipmentActionState.Empty)
			{
				return EquipmentUseResult.NoAmmoInMagazine;
			}

			if (Magazine <= 0)
			{
				State = EquipmentActionState.Empty;
				OnMagazineEmpty?.Invoke();
				return EquipmentUseResult.NoAmmoInMagazine;
			}

			Magazine--;

			this.UseDelay = InitialUseDelay;
			State = EquipmentActionState.UseDelaying;
			Accuracy += AccuracyIncrease;
			if (Accuracy > MaxAccuracy)
			{
				Accuracy = MaxAccuracy;
			}

			return EquipmentUseResult.Success;
		}
	}
}
