using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.ViewModel;

public class Test_ScreenLoader : MonoBehaviour
{
	public ButtonViewModel AsyncButton;

	public RawImage Background;

    public void Awake()
    {
	}

    async void Start()
	{
		var desktopBackgroundTexture = await ImageProcesser.LoadDesktopBackgroundTextureOrNull(240, 135);

		if (desktopBackgroundTexture != null)
		{
			Background.texture = desktopBackgroundTexture;
		}

		AsyncButton.Initialize(this);
		AsyncButton.BindAction(SomeFunction);

	}

	public async void SomeFunction()
    {
		Debug.Log("Some Function");

		int value = await SomeAsync();

		var taskOperation = LoadPathAsync();

	}

	public async Task<int> SomeAsync()
    {
		await Task.Delay(1000);

		return 100;
    }

	public async Task<string> LoadPathAsync()
    {
		await Task.Delay(100);

		return "fsfse";
    }
}