using UnityEngine;
using Utils.ViewModel;

namespace MonoGUI
{
	public class View_TitleMenu : MonoGUI_View
	{
		//#if UNITY_EDITOR

		//		private bool Btn_GameStartButton_IsBindable() => Btn_GameStartButton.IsBindable(this);
		//		private bool Btn_GameOptionButton_IsBindable() => Btn_GameOptionButton.IsBindable(this);
		//		private bool Btn_GameExitButton_IsBindable() => Btn_GameExitButton.IsBindable(this);

		//#endif

		//[ValidateInput("Btn_GameStartButton_IsBindable", "게임 시작 버튼이 존재하지 않습니다.")]
		//[SerializeField] private ButtonViewModel Btn_GameStartButton = new(nameof(Btn_GameStartButton));

		//[ValidateInput("Btn_GameOptionButton_IsBindable", "게임 옵션 버튼이 존재하지 않습니다.")]
		//[SerializeField] private ButtonViewModel Btn_GameOptionButton = new(nameof(Btn_GameOptionButton));

		//[ValidateInput("Btn_GameExitButton_IsBindable", "게임 종료 버튼이 존재하지 않습니다.")]
		//[SerializeField] private ButtonViewModel Btn_GameExitButton = new(nameof(Btn_GameExitButton));


		[SerializeField] private ButtonViewModel Btn_ServerBrowser = new(nameof(Btn_ServerBrowser));
		[SerializeField] private ButtonViewModel Btn_CreateLobby = new(nameof(Btn_CreateLobby));
		[SerializeField] private ButtonViewModel Btn_SinglePlay = new(nameof(Btn_SinglePlay));
		[SerializeField] private ButtonViewModel Btn_Option = new(nameof(Btn_Option));
		[SerializeField] private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));

		[SerializeField] private Navigation_TitleMenu_Window Navigation_TitleMenuWindow;

		public override void OnInitialized()
		{
			#region Old Code
			//		GlobalServiceLocator.NetworkManageService
			//.GetServiceOrNull()
			//.JoinToHideout();
			//Btn_GameOptionButton.Initialize(this);
			//Btn_GameOptionButton.BindAction(() =>
			//{
			//	//GlobalServiceLocator.SceneManageService
			//	//	.GetServiceOrNull<TitleSceneManageService>()
			//	//	.TitleGuiService
			//	//	.OnGUI_GotoOption();
			//	OptionWindow.OnOpenOption();
			//});

			//Btn_GameExitButton.Initialize(this);
			//Btn_GameExitButton.BindAction(() =>
			//{
			//	ProcessHandler.Instance.StopProcess();
			//});
			#endregion

			Navigation_TitleMenuWindow.Initialize();

			//GlobalServiceLocator.NetworkManageService
			//	.GetServiceOrNull()
			//	.JoinToHideout();

			Btn_ServerBrowser.Initialize(this);
			Btn_ServerBrowser.BindAction(() =>
			{
				Navigation_TitleMenuWindow.OpenServerBrowser();
			});

			Btn_CreateLobby.Initialize(this);
			Btn_CreateLobby.BindAction(() =>
			{
				Navigation_TitleMenuWindow.OpenCreateLobby();
			});

			Btn_SinglePlay.Initialize(this);
			Btn_SinglePlay.BindAction(() =>
			{
				Navigation_TitleMenuWindow.OpenSinglePlay();
			});

			Btn_Option.Initialize(this);
			Btn_Option.BindAction(() =>
			{
				GlobalServiceLocator.GlobalGuiService.GetServiceOrNull().OpenSetting();
			});

			Btn_Exit.Initialize(this);
			Btn_Exit.BindAction(() =>
			{
				ProcessHandler.Instance.StopProcess();
			});
		}
	}

}
