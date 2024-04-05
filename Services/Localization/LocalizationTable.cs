using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public static class LocalizationTable
{

	private static Dictionary<LocalizationType, LocaleIdentifier> mLocaleIdentifierTable = new()
	{
		{LocalizationType.KR, new LocaleIdentifier("ko")},
		{LocalizationType.KR, new LocaleIdentifier("en")},
	};

	public static LocaleIdentifier GetLocaleIdentifier(LocalizationType type)
	{
		return mLocaleIdentifierTable[type];
	}
}
