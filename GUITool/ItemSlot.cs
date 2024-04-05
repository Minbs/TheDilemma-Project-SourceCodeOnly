using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;

public class ItemSlot : MonoBehaviour
{
	[SerializeField] private ImageViewModel Image_Icon = new(nameof(Image_Icon));
	[SerializeField] private TextMeshProTextViewModel Text_Name = new(nameof(Text_Name));
	[SerializeField] private TextMeshProTextViewModel Text_HotKey = new(nameof(Text_HotKey));
	[SerializeField] private ImageViewModel Img_CoolTime = new(nameof(Img_CoolTime));
	
	public EquipmentState State { get; private set; } 

	private StrachHorizontalImage mCoolTimeImage;
	private const float mMax = 1.0f;
	public bool IsCoolTimeAnimationRunning { get; private set; } = false;

	public void Initialized(Sprite icon, string name, string hotKey, EquipmentState state)
	{
		Image_Icon.Initialize(this);
		Text_Name.Initialize(this);
		Text_HotKey.Initialize(this);
		Img_CoolTime.Initialize(this);
		mCoolTimeImage = new(Img_CoolTime, 0.0f, 0.0f);

		State = state;

		Image_Icon.Sprite = icon;
		Text_Name.Text = name;
		Text_HotKey.Text = hotKey;

		gameObject.SetActive(true);
	}

	public void CoolTime(float percent)
	{
		mCoolTimeImage.Max = percent;
	}

}
