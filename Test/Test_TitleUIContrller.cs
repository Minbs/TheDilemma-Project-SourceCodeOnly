using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Test_TitleUIContrller : MonoBehaviour
{
	public Button Btn_StartTheGame;
	public Button Btn_ExitTheGame;

	public void Awake()
	{
		Btn_StartTheGame.onClick.AddListener(OnStartTheGame);
		Btn_ExitTheGame.onClick.AddListener(OnExitTheGame);
	}

	public void OnStartTheGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(SceneType.scn_game_hideout.ToString());
	}

	public void OnExitTheGame()
	{
		ProcessHandler.Instance.StopProcess();
	}
}
