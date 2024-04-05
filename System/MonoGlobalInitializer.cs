using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Utils;
using UnityEngine;
using Utils;

/// <summary>게임 초기화를 위한 모노 클래스입니다. 모노 객체를 초기화합니다.</summary>
public class MonoGlobalInitializer : MonoBehaviour
{
	private List<IProcessInitializable> mMonoInitializeList = new();

	private void Awake()
	{
	}

	public void InitializeByProcessHandler(ProcessHandler gameProcessHandler)
	{
		var monoObjects = GetComponentsInChildren<IProcessInitializable>();
		mMonoInitializeList.AddRange(monoObjects);

		foreach (var i in mMonoInitializeList)
		{
			i.InitializeByProcessHandler(gameProcessHandler);
			gameProcessHandler.AddProcessInitialMessage($"{i.GetType().Name} initialized!");
		}
	}
}
