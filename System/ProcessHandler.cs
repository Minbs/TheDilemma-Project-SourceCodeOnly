using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;
using KaNet.Synchronizers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

/// <summary>게임 프로세스를 컨트롤하는 Mono클래스입니다.</summary>
public class ProcessHandler : MonoBehaviour
{
	[field : SerializeField] public bool IsDebugModeSetup { get; private set; } = false;
	public static bool IsDebugMode { get; set; } = false;

	/// <summary>프로세스 컨트롤러의 인스턴스입니다.</summary>
	public static ProcessHandler Instance => mInstance;
	private static ProcessHandler mInstance;

	private MonoGlobalInitializer mMonoGlobalInitializer;
	public InitializeLogPanel LogPanel;

	public NetProgramID ID { get; private set; }

	public event Action OnStopProcess;

	private static bool mIsQuitted = false;

	public string SystemPath { get; private set; } = Directory.GetCurrentDirectory();
	public string ErrorLogPath => SystemPath + "/ErrorLog.txt";

	public object Message = new object();

	private void Start()
	{
		AddProcessInitialMessage("Start initialize process!");
		// Set debug mode
		IsDebugMode = IsDebugModeSetup;

		// Set System Path
		//SystemPath = Application.dataPath;
		//if (Application.platform == RuntimePlatform.OSXPlayer)
		//{
		//	SystemPath += "/../../";
		//}
		//else if (Application.platform == RuntimePlatform.WindowsPlayer)
		//{
		//	SystemPath += "/../";
		//}

		// Setup Singleton
		DontDestroyOnLoad(this);
		mInstance = this;

		// Setup Process Identification
		this.ID = new NetProgramID(
			Application.productName,
			Application.version,
			480);

		// Setup Initial GameObject
		mMonoGlobalInitializer = GetComponentInChildren<MonoGlobalInitializer>();
		if (mMonoGlobalInitializer == null)
		{
			AddProcessInitialMessage("There is no MonoGlobalInitialize!");
			return;
		}

		// Start Process
		string[] processCommandArguments = null;

		try
		{
			processCommandArguments = System.Environment.GetCommandLineArgs();
		}
		catch (Exception e)
		{
			AddProcessInitialMessage("Environmnet command getting error!", true);
			AddProcessInitialMessage($"{e.Message}\n", true);
			return;
		}

		AddProcessInitialMessage("Command line initialized.");
		Ulog.Initialize(new UnityLogger());
		AddProcessInitialMessage("Ulog initialized...");
		StartProcess(processCommandArguments);
	}

	/// <summary>프로세스를 시작합니다.</summary>
	/// <param name="args">프로세스 실행 인자</param>
	public void StartProcess(string[] args)
	{
		AddProcessInitialMessage("Start process...");
		Ulog.Log(UlogType.Process, $"Game process started!");
		AddProcessInitialMessage("First Ulog printed...");

		// Check Arguemtns
		List<string> argumentsList = new List<string>(args);
		if (argumentsList.Contains("-SomeKindOfOption")) { } // Do something
		AddProcessInitialMessage("Command arguments check finished...");

		// Initialize Global States
		try
		{
			GlobalInitializer.InitializeByProcessHandler(this);
			AddProcessInitialMessage($"Static global initialize finished!");

			var monoObjects = GetComponentsInChildren<IProcessInitializable>();

			foreach (var i in monoObjects)
			{
				i.InitializeByProcessHandler(this);
				AddProcessInitialMessage($"{i.GetType().Name} initialized!");
			}

			AddProcessInitialMessage($"Mono global initialize finished!");

			// Change Nextscene
			bool? initialLoad = GlobalServiceLocator.AsyncSceneLoadService
				.GetServiceOrNull()?
				.TryLoadSceneAsync(SceneType.scn_game_title);

			AddProcessInitialMessage($"Try to change scene");

			if (!initialLoad.HasValue || initialLoad.Value == false)
			{
				Ulog.LogError(UlogType.Process, $"Initial scene loading failed!");
				AddProcessInitialMessage($"[{UlogType.Process}] Initial scene loading failed!", true);
				return;
			}
		}
		catch (Exception e)
		{
			AddProcessInitialMessage($"{e.Message}", true);
			return;
		}
	}

	public void AddProcessInitialMessage(string message, bool isError = false)
	{
		if (isError)
		{
			Ulog.LogError(this, message);
		}
		else
		{
			Ulog.Log(this, message);
		}

		lock (Message)
		{
			LogPanel.AddLogMessage(message);
		}
	}

	/// <summary>프로세스를 중단합니다.</summary>
	public void StopProcess()
	{
		if (mIsQuitted)
		{
			return;
		}

		mIsQuitted = true;

		try
		{
			Ulog.Log(UlogType.Process, "Try to stop process...");
			OnStopProcess?.Invoke();
			Ulog.Log(UlogType.Process, "Process stop successful!");
		}
		catch (Exception e)
		{
			Ulog.LogError(UlogType.Process, $"[EXTRIME SERIOUS ERROR OCCUR] Process stop operation fail!\n{e}");
		}
		finally
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}

	public void OnDisable()
	{
		StopProcess();
	}

	public void OnDestroy()
	{
		StopProcess();
	}

	public void OnApplicationQuit()
	{
		StopProcess();
	}
}
