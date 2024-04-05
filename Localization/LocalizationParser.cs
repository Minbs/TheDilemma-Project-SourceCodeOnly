using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Utils;

#if UNITY_EDITOR
public class LocalizationParser : EditorWindow
{

	[MenuItem("Localization/Import Data")]
	public static void OnImportLocalizationData()
	{
		var path = Application.dataPath + "/Localization/TestLanguage.csv";
		char[] filter = { ',', '\n' };
		string tableName = "LocalizationTable";
		try
		{
			Ulog.Log("Improting localization.....");
			using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("ks_c_5601-1987"), true))
			{
				List<string> languageList = new();
				List<StringTable> languageTableList = new();

				string line;
				string labelLine = sr.ReadLine();

				var labels = labelLine.Split(filter);
				int langeuageCount = 0;

				//사용가능한 언어 타입인지 확인
				for (int i = 1; i < labels.Length; i++)
				{
					LocaleIdentifier identifier = new LocaleIdentifier(labels[i]);
					if (!IsAvailableLocal(identifier))
					{
						throw new Exception($"This language code {labels[i]} is not set");
					}
					var local = Locale.CreateLocale(identifier);
					var table = LocalizationSettings.StringDatabase.GetTable(tableName, local);
					languageTableList.Add(table);

					langeuageCount++;
				}

				//테이블에 데이터 넣기
				while ((line = sr.ReadLine()) != null)
				{
					var datas = line.Split(filter);
					var key = datas[0];

					var log = $"AddEntry key : {key} data : ";
					for (int i = 0; i < langeuageCount; i++)
					{
						var localized = datas[i + 1];
						log += $"|{localized}";
						languageTableList[i].AddEntry(key, localized);
					}
					log += "|";
					Ulog.Log(log);
				}

				foreach(StringTable table in languageTableList)
				{
					EditorUtility.SetDirty(table);
					EditorUtility.SetDirty(table.SharedData);
				}

				Ulog.Log("done.");
			}
		}
		catch (Exception e)
		{
			Ulog.LogError($"Localization Error {e.Message}");
		}
	}

	private static bool IsAvailableLocal(LocaleIdentifier identifier)
	{
		foreach (var local in LocalizationSettings.AvailableLocales.Locales)
		{
			if (identifier.Equals(local.Identifier))
				return true;
		}
		return false;
	}

}

#endif