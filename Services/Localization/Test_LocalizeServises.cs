using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;
public class Test_LocalizeServises : MonoBehaviour
{
	[SerializeField]
	private TextMeshProTextViewModel Text_TestA = new(nameof(Text_TestA));
	[SerializeField]
	private TextMeshProTextViewModel Text_TestB = new(nameof(Text_TestB));


	private LocalizationText mLocalTextA;
	private LocalizationText mLocalTextB;

	private LocalizationService mLocalService;

	private void Start()
	{
		mLocalService = GlobalServiceLocator.LocalizationService.GetServiceOrNull();
		Text_TestA.Initialize(this);
		Text_TestB.Initialize(this);

		mLocalTextA = new(Text_TestA, "K_A");
		mLocalTextB = new(Text_TestB, "K_B");

	}

	public void SwitchKR()
	{
		mLocalService.SwitchLanguage(LocalizationType.KR);
	}

	public void switchEN()
	{
		mLocalService.SwitchLanguage(LocalizationType.EN);
	}
}
