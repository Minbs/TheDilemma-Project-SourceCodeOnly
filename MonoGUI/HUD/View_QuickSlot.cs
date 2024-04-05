using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoGUI;
using Utils;
using Utils.ViewModel;
using Gameplay;

public struct ItemSlotData
{
	public Sprite Icon;
	public string Name;
	public KeyCode Key;
	public EquipmentState State;
	public ItemSlotData(Sprite icon, string name, KeyCode key, EquipmentState state)
	{
		Icon = icon;
		Name = name;
		Key = key;
		State = state;
	}
}

public class View_QuickSlot : MonoGUI_View
{
	[SerializeField] private ItemSlot mSlotContent;
	[SerializeField] private TransformViewModel Pivot_SlotPanel = new(nameof(Pivot_SlotPanel));

	private List<ItemSlot> mSlotList = new();
	private List<ItemSlot> mShowSlotList = new();

	private int mSlotCount = 3;

	private GameplayManager mGameplayManager;

	[SerializeField] SerializableDictionary<EquipmentType, Sprite> mEquipmentSpriteList = new();


	public override void OnInitialized()
	{
		Pivot_SlotPanel.Initialize(this);
	}

	private IEnumerator lateInitialize()
	{
		yield return new WaitUntil(() =>
		{
			return mGameplayManager.EntityService.PlayerEntityService.IsClientPlayerExist;
		});

		if (!mGameplayManager.EntityService.PlayerEntityService.TryGetClientPlayer(out var player))
		{
			yield break;
		}

		player.TryGetPrimaryState(out var primaryState);
		player.TryGetSecondaryState(out var secondaryState);
		player.TryGetAuxilliaryState(out var auxillaryState);

		EquipmentType primary = primaryState.InitialData.Equipment.GetEnum();
		EquipmentType secondary = secondaryState.InitialData.Equipment.GetEnum();
		EquipmentType auxillary = auxillaryState.InitialData.Equipment.GetEnum();

		ItemSlotData[] datas =
		{
			new ItemSlotData(
				mEquipmentSpriteList[primary],
				primary.ToString(),
				KeyCode.Alpha1,
				primaryState),
			new ItemSlotData(
				mEquipmentSpriteList[secondary],
				secondary.ToString(),
				KeyCode.Alpha2,
				secondaryState),
			new ItemSlotData(
				mEquipmentSpriteList[auxillary],
				auxillary.ToString(),
				KeyCode.Alpha3,
				auxillaryState),
		};

		while (mShowSlotList.Count > 0)
		{
			mShowSlotList[0].gameObject.SetActive(false);
			mShowSlotList.RemoveAt(0);
		}

		for (int i = 0; i < datas.Length; i++)
		{
			Sprite sprite = datas[i].Icon;
			string name = datas[i].Name;
			string key = datas[i].Key.ToString().Split("Alpha")[1];
			EquipmentState state = datas[i].State;

			ItemSlot item = null;

			if (i < mSlotList.Count)
			{
				mSlotList[i].Initialized(sprite, name, key, state);
				item = mSlotList[i];
			}
			else
			{
				item = createSlot();
				item.Initialized(sprite, name, key, state);
			}

			mShowSlotList.Add(item);
		}
	}

	public void Initilaized(GameplayManager gameplayManager)
	{
		int slotCount = 0;

		mGameplayManager = gameplayManager;

		StartCoroutine(lateInitialize());
	}

	private ItemSlot createSlot()
	{
		return Instantiate(mSlotContent, Pivot_SlotPanel.Transform);
	}

	private void FixedUpdate()
	{
		if (!mGameplayManager.EntityService.PlayerEntityService.IsClientPlayerExist)
			return;

		foreach (var slot in mShowSlotList)
		{
			slotUpdate(slot);
		}

	}

	private void slotUpdate(ItemSlot slot)
	{
		slot.CoolTime(0.0f);

		switch (slot.State.State)
		{
			case EquipmentActionState.Empty:
				slot.CoolTime(1.0f);
				break;
			case EquipmentActionState.Reloading:
				slot.CoolTime(slot.State.ReloadProgress);
				break;
		}

	}

}
