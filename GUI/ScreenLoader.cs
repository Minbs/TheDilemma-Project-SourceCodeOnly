using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utils.ViewModel;

public class ScreenLoader : MonoBehaviour
{
	[SerializeField]
	private RawImageViewModel Img_OsBackground = new(nameof(Img_OsBackground));

	void Start()
	{
		Img_OsBackground.Initialize(this);

		var screenImage = GlobalServiceLocator.ResourcesService
			.GetServiceOrNull()?
			.TerminalScreen;

		Img_OsBackground.Texture = screenImage ?? Img_OsBackground.Texture;
	}
}