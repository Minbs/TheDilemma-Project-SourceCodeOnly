using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_DebugLogPanel : MonoGUI_View
	{
		public TextMeshProTextViewModel Text_DebugLogPanel = new TextMeshProTextViewModel(nameof(Text_DebugLogPanel));

		public string mLogStream = "";
		int MaxChars = 12000;

		void OnEnable()
		{
			Application.logMessageReceived += Log;
		}

		void OnDisable()
		{
			Application.logMessageReceived -= Log;
		}

		public void Log(string logString, string stackTrace, LogType type)
		{
			// for onscreen...
			mLogStream = mLogStream + "\n" + logString;

			if (mLogStream.Length > MaxChars)
			{
				mLogStream = mLogStream.Substring(mLogStream.Length - MaxChars);
			}

			Text_DebugLogPanel.Text = mLogStream;
			//// for the file ...
			//if (filename == "")
			//{
			//	string d = System.Environment.GetFolderPath(
			//	   System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
			//	System.IO.Directory.CreateDirectory(d);
			//	string r = Random.Range(1000, 9999).ToString();
			//	filename = d + "/log-" + r + ".txt";
			//}
			//try { System.IO.File.AppendAllText(filename, logString + "\n"); }
			//catch { }
		}

		public override void OnInitialized()
		{
			Text_DebugLogPanel.Initialize(this);
		}
	}
}
