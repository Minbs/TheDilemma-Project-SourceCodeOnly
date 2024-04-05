public enum GUINavigationType
{
	None = 0,

	// 테스트에 사용되는 타입입니다.
	_InGame,
	_Option,
	_Title,
	_Loadout,
	Test,

	// 아무것도 없는 네비게이션입니다.
	Null = 100,

	// 전역 네비게이션은 게임 내부에서 모두 사용할 수 있는 네비게이션입니다.
	Global = 1000,

	/// <summary>전역 게임 종료 네비게이션입니다.</summary>
	Global_ExitGame = 1100,

	/// <summary>전역 옵션 네비게이션입니다.</summary>
	Global_Option = 1500,

	/// <summary>개발자 크레딧 네비게이션입니다.</summary>
	Global_Credits = 1900,


	// 타이틀 네비게이션은 게임 타이틀에서 사용할 수 있는 네비게이션입니다.
	Title = 2000,

	/// <summary>메인 타이틀 네비게이션입니다.</summary>
	Title_Main = 2100,

	// 로비 네비게이션은 로비에서 사용할 수 있는 네비게이션입니다.
	Lobby = 2500,

	/// <summary>로비의 메인 타이틀입니다.</summary>
	Lobby_Main = 2600,

	// 인게임 네비게이션은 로비와 하이드아웃을 포함한 게임 내부에서 사용할 수 있는 네비게이션입니다.
	InGame = 3000,

	/// <summary>게임을 중단하고 은신처로 돌아가는 UI 네비게이션입니다.</summary>
	InGame_GoBackToHideout = 3100,

	/// <summary>게임의 HUD 화면입니다. 관전 카메라를 포함합니다.</summary>
	InGame_HUD = 3200,

	/// <summary>게임 로드아웃 네비게이션입니다.</summary>
	InGame_Loadout = 3400,

	/// <summary>게임 상점 네비게이션입니다.</summary>
	InGame_Shop = 3600,

	/// <summary>유저의 창고 네비게이션입니다.</summary>
	InGame_Stash = 3800,

	/// <summary>캐릭터 선택에 사용되는 네비게이션입니다.</summary>
	InGame_CharacterSelector = 3900,

	/// <summary>은신처에서 서버 접속 및 싱글플레이 시작에 사용되는 컴퓨터 UI 네비게이션입니다.</summary>
	InGame_HideoutTerminal = 4000,

	/// <summary>로비에서 친구 초대 및 서버의 설정 및 정보를 보는데 사용되는 컴퓨터 UI 네비게이션입니다.</summary>
	InGame_LobbyTerminal = 4100,

	/// <summary> 환경설정 네비게이션입니다. </summary>
	InGame_Preferences = 4200,
}


