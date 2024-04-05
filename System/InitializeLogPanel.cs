using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class InitializeLogPanel : MonoBehaviour
{
	[field : SerializeField] public TextMeshProUGUI InitializeLogText { get; private set; }
	[field : SerializeField] public TextMeshProUGUI ErrorLogPath { get; private set; }

	public void Start()
	{
		AddLogMessage($"{typeof(InitializeLogPanel).Name}");
		ErrorLogPath.text = Directory.GetCurrentDirectory();
	}

	public void AddLogMessage(string message)
	{
		InitializeLogText.text = InitializeLogText.text + message + '\n';
	}
}