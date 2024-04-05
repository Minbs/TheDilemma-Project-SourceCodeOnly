using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Service;

[Obsolete("직교 카메라를 더 이상 쓰지 않습니다.")]
public struct ScreenInfo3D
{
	public const float PPU = 32.0F;

	public int ScreenWidth;
	public int ScreenHeight;

	public float CameraViewWidth;
	public float CameraViewHeight;

	public float Scale;

	public Matrix4x4 OrthoMatrix;

	public ScreenInfo3D(int width, int height)
	{
		ScreenWidth = width;
		ScreenHeight = height;

		Scale = (ScreenHeight / 270) * 0.5f;

		CameraViewWidth = (ScreenWidth / PPU) / Scale;
		CameraViewHeight = (ScreenHeight / PPU) / Scale;

		var hw = CameraViewWidth * 0.5f;
		var hh = CameraViewHeight * 0.5f / Mathf.Sqrt(2);

		OrthoMatrix = Matrix4x4.Ortho(-hw, hw, -hh, hh, 0.3f, 1000);
	}
}

public class ScreenInfo
{
	public const float PPU = 32.0F;

	public int ScreenWidth { get; private set; }
	public int ScreenHeight { get; private set; }
	public float Scale { get; private set; }

	public ScreenInfo(float width, float height)
	{
		SetByResolution((int)width, (int)height);
	}

	public ScreenInfo(int width, int height)
	{
		SetByResolution(width, height);
	}

	public void SetByResolution(int width, int height)
	{
		ScreenWidth = width;
		ScreenHeight = height;

		Scale = (ScreenHeight / 270) * 0.5f;
	}
}

public class OptionService : MonoService
{
	public event Action<ScreenInfo> OnResolutionChanged;

	public ScreenInfo ScreenInfo { get; private set; }

	public void SetResolution(int width, int height, FullScreenMode fullScreenMode, int fps)
	{
		Screen.SetResolution(width, height, fullScreenMode, fps);
	}

#if UNITY_EDITOR
	private Func<Vector2> mEditorResolutionGetter;
#endif

	private int mScreenWidth = 0;
	private int mScreenHeight = 0;

	public void FixedUpdate()
	{
		int curScreenWidth = 0;
		int curScreenHeight = 0;

#if UNITY_EDITOR
		var currentScreen = mEditorResolutionGetter();
		curScreenWidth = (int)currentScreen.x;
		curScreenHeight = (int)currentScreen.y;
#else
		curScreenWidth = Screen.width;
		curScreenHeight = Screen.height;
#endif
		if (curScreenWidth != mScreenWidth && curScreenHeight != mScreenHeight)
		{
			ScreenInfo = new ScreenInfo(curScreenWidth, curScreenHeight);
			OnResolutionChanged?.Invoke(ScreenInfo);
			mScreenWidth = curScreenWidth;
			mScreenHeight = curScreenHeight;
		}
	}

public override void OnRegistered()
	{
		base.OnRegistered();

#if UNITY_EDITOR
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		var getter = T.GetMethod
		(
			"GetSizeOfMainGameView",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
		);

		mEditorResolutionGetter = () => (Vector2)getter.Invoke(null, null);

		var screenSize = mEditorResolutionGetter();

		ScreenInfo = new ScreenInfo(screenSize.x, screenSize.y);
#else
		ScreenInfo = new ScreenInfo(Screen.width, Screen.height);
#endif

		mScreenWidth = ScreenInfo.ScreenWidth;
		mScreenHeight = ScreenInfo.ScreenHeight;

		OnResolutionChanged?.Invoke(ScreenInfo);
	}

	public override void OnUnregistered()
	{
		base.OnUnregistered();
	}
}
