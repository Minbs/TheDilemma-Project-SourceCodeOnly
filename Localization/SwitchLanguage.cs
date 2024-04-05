using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public enum LanguageType
{
	KR,
	EN
}

public class SwitchLanguage : MonoBehaviour
{
	private Dictionary<LanguageType, LocaleIdentifier> mLanguageTable = new()
	{
		{LanguageType.KR, new LocaleIdentifier("ko")},
		{LanguageType.EN, new LocaleIdentifier("en") }
	};

	public void Start()
	{
		Locale kr = Locale.CreateLocale(mLanguageTable[LanguageType.KR]);
		var table = LocalizationSettings.StringDatabase.GetTable("Test", kr);
	}



	public void OnSwitchLanguage(LanguageType type)
	{
		foreach(var local in LocalizationSettings.AvailableLocales.Locales)
		{
			if (mLanguageTable[type].Equals(local.Identifier))
			{
				LocalizationSettings.SelectedLocale = local;
				break;
			}
		}
	}



	public void OnSwitchKR()
	{
		OnSwitchLanguage(LanguageType.KR);
	}

	public void OnSwitchEN()
	{
		OnSwitchLanguage(LanguageType.EN);
	}


	public void TestSkip()
	{
		SceneManager.LoadScene(0);
	}
}
