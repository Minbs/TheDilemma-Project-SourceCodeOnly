using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class EditorGlobal
{
	/// <summary>현재 프로젝트의 절대 경로</summary>
	public static string ApplicationDataPath => Application.dataPath.Replace("/", "\\");
	/// <summary>Resources 폴더의 경로</summary>
	public static string ResourcesDataPath => ApplicationDataPath + $@"\Resources\KaNet";

	/// <summary>현재 프로젝트의 경로로 부터의 절대 경로를 얻습니다.</summary>
	/// <param name="relativePath">프로젝트 경로로 부터의 상대 경로</param>
	/// <returns>절대 경로</returns>
	public static string GetPathOnApplicationDataPath(string relativePath)
	{
		return ApplicationDataPath + relativePath;
	}

	/// <summary>리소스 폴더로 부터의 절대 경로를 얻습니다.</summary>
	/// <param name="relativePath">리소스 경로로 부터의 상대 경로</param>
	/// <returns>절대 경로</returns>
	public static string GetPathOnResourcesDataPath(string relativePath)
	{
		return ResourcesDataPath + relativePath;
	}

	/// <summary>해당 위치의 Prefab을 불러옵니다.</summary>
	/// <param name="folderPathInAsset">폴더 위치</param>
	/// <param name="containContext">포함되어야 하는 문자열 리스트</param>
	/// <returns>Prefab 리스트</returns>
	public static List<GameObject> GetPrefabsFileFromPath(string folderPathInAsset, List<string> containContext = null)
	{
		var filePaths = Directory.GetFiles(folderPathInAsset);
		List<GameObject> loadedPrefabs = new List<GameObject>();

		foreach (string filePath in filePaths)
		{
			bool isNotMatch = false;

			if (containContext != null)
			{
				foreach (string context in containContext)
				{
					if (!string.IsNullOrEmpty(context) && filePath.ToLower().Contains(context.ToLower()) == false)
					{
						isNotMatch = true;
						break;
					}
				}
			}

			if (isNotMatch)
				continue;

			string currentFilePath = filePath.Replace(ApplicationDataPath, "Assets");

			if (IsPrefabFile(currentFilePath) && TryLoadPrefabFromFilePath(currentFilePath, out var loadedPrefab))
			{
				loadedPrefabs.Add(loadedPrefab);
			}
		}

		return loadedPrefabs;
	}


	public static bool TryLoadPrefabFromFilePath(string prefabFilePath, out GameObject prefabObject)
	{
		prefabObject = AssetDatabase.LoadAssetAtPath(prefabFilePath, typeof(GameObject)) as GameObject;
		return prefabObject != null;
	}

	public static bool IsPrefabFile(string prefabFilePath)
	{
		string fileExtension = Path.GetExtension(prefabFilePath).ToLower();
		return fileExtension == ".prefab";
	}
}
#endif