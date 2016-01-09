using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;

public class ButlerDemo : MonoBehaviour,IButler
{

	public event Game.GameStateChange OnGameStateChange;

	bool playing;

	public int NumPlayers ()
	{
		return playing ? 2 : 0;
	}

	public string GetLocalUsername ()
	{
		return playing ? "Ford Prefect" : "";
	}

	public void Init ()
	{
	}

	public void NewGame ()
	{
		Assert.IsFalse (playing);
		playing = true;
		OnGameStateChange (GameState.PLAYING);
	}

	public GameState GetGameState ()
	{
		return playing ? GameState.PLAYING : GameState.SELECTING_GAME_TYPE;
	}

	public void QuitGame ()
	{
		Assert.IsTrue (playing);
		playing = false;
		OnGameStateChange (GameState.SELECTING_GAME_TYPE);
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		SceneMaster.instance.Async (delegate {
			Game.instance.OnRealTimeMessageReceived (reliable, "dummySenderId", data);
		}, Utils.DUMMY_PLAY_GAMES_REPLAY_DELAY);
	}

	public string ToString ()
	{
		return string.Format ("[ButlerDemo: playing={0}]", playing);
	}
}
