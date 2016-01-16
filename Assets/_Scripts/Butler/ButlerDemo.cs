using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;

public class ButlerDemo : MonoBehaviour,IButler
{

	public event Game.GameStateChange OnGameStateChange;

	GameState gameState = GameState.NEED_TO_SELECT_GAME_TYPE;

	#if UNITY_EDITOR
	void Update ()
	{
		if (gameState == GameState.PLAYING && Input.GetKeyDown (KeyCode.F)) {
			Debug.Log ("***Simulating failure …");
			QuitGame ();
		}
	}
	#endif

	public int NumPlayers ()
	{
		return gameState == GameState.PLAYING ? 2 : 0;
	}

	public string GetLocalUsername ()
	{
		return gameState == GameState.PLAYING ? "Ford Prefect" : "";
	}

	public void Init ()
	{
	}

	public void NewGame ()
	{
		Assert.AreEqual (GameState.NEED_TO_SELECT_GAME_TYPE, gameState);
		SetGameState (GameState.AUTHENTICATING);
		SetGameState (GameState.SETTING_UP_GAME);
		SetGameState (GameState.PLAYING);
	}

	public GameState GetGameState ()
	{
		return gameState;
	}

	public void QuitGame ()
	{
		Assert.AreEqual (GameState.PLAYING, gameState);
		SetGameState (GameState.TEARING_DOWN_GAME);
		SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		SceneMaster.instance.Async (delegate {
			Game.instance.OnRealTimeMessageReceived (reliable, "dummySenderId", data);
		}, data [0] == (byte)'G' ? .1f : Utils.DUMMY_PLAY_GAMES_REPLAY_DELAY);
	}

	void SetGameState (GameState gameState)
	{
		this.gameState = gameState;
		OnGameStateChange (gameState);
		if (gameState == GameState.GAME_WAS_TORN_DOWN) {
			SetGameState (GameState.NEED_TO_SELECT_GAME_TYPE);
		}
	}

	public override string ToString ()
	{
		return string.Format ("[ButlerDemo: gameState={0}]", gameState);
	}
}
