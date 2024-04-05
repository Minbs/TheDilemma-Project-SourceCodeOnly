using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceSceneChanger : MonoBehaviour
{
	public void GotoSoundIngameScene()
	{
		SceneManager.LoadScene("sound_ingame");
	}

	public void GotoLocalTestScene()
	{
		SceneManager.LoadScene("LocalizationTest");
	}

	public void GotoAnimationTestScene()
	{
		SceneManager.LoadScene("AnimationTest");
	}
}
