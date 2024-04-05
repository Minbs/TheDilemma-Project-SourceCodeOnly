
/// <summary>게임의 전역 상태입니다.</summary>
public enum GameStateType : byte
{
	None = 0,
	Hideout,
	/// <summary>로비 상태입니다.</summary>
	Lobby,
	/// <summary>이벤트 상태입니다.</summary>
	Event,
	/// <summary>스테이지 플레이중입니다.</summary>
	Stage,
	/// <summary>서버가 로딩중입니다.</summary>
	ServerLoading,
}

//public class GameState
//{
//	public int Level = 0;
//}

public class GameplayData
{
	public GameStateType CurrentGameState;

}