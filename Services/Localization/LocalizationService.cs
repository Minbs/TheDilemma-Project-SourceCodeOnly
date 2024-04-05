using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Utils;
using Utils.Service;

public class LocalizationService : IServiceable
{
	public void OnRegistered()
	{
		Ulog.Log(this, "OnRegistered");
	}

	public void OnUnregistered()
	{
		Ulog.Log(this, "OnUnregistered");
	}

	public void SwitchLanguage(LocalizationType type)
	{
		LocaleIdentifier findingLocal = LocalizationTable.GetLocaleIdentifier(type);

		foreach (var local in LocalizationSettings.AvailableLocales.Locales)
		{
			if (findingLocal.Equals(local.Identifier))
			{
				LocalizationSettings.SelectedLocale = local;
				break;
			}
		}
	}

}
