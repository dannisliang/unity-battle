using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;
using System;

public class ButlerAi : MonoBehaviour,IButler
{
	static int gameCount;

	public event Game.GameStateChange OnGameStateChange;

	GameState gameState = GameState.SELECTING_GAME_TYPE;
	GameAi ai;

	#if UNITY_EDITOR
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			if (gameState == GameState.SELECTING_VIEW_MODE || gameState == GameState.PLAYING) {
				Debug.Log ("***Simulating failure …");
				QuitGame ();
			}
		}
	}
	#endif

	public int NumPlayers ()
	{
		return (gameState == GameState.SELECTING_VIEW_MODE || gameState == GameState.PLAYING) ? 2 : 0;
	}

	public string GetLocalUsername ()
	{
		return (gameState == GameState.SELECTING_VIEW_MODE || gameState == GameState.PLAYING) ? "Ford Prefect" : "";
	}

	public void Init ()
	{
	}

	public void NewGame ()
	{
		gameCount++;
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, gameState);
		SetGameState (GameState.AUTHENTICATING);
		SetGameState (GameState.SETTING_UP_GAME);
		SetGameState (GameState.SELECTING_VIEW_MODE);
	}

	public void StartGamePlay ()
	{
		Assert.AreEqual (GameState.SELECTING_VIEW_MODE, gameState);
		SetGameState (GameState.PLAYING);
		if (ai != null) {
			Destroy (ai);
		}
		ai = gameObject.AddComponent<GameAi> ();
	}

	public void PauseGamePlay ()
	{
		Assert.AreEqual (GameState.PLAYING, gameState);
		SetGameState (GameState.SELECTING_VIEW_MODE);
	}

	public GameState GetGameState ()
	{
		return gameState;
	}

	public void QuitGame ()
	{
		if (gameState == GameState.PLAYING || gameState == GameState.SELECTING_VIEW_MODE) {
			SetGameState (GameState.TEARING_DOWN_GAME);
		}
		if (gameState == GameState.TEARING_DOWN_GAME) {
			SetGameState (GameState.GAME_WAS_TORN_DOWN);
			Destroy (ai);
		}
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		if (!enabled) {
			Debug.Log ("***INGORING SendMessageToAll() as " + name + " is disabled");
			return;
		}
		byte[] replyData = MakeReply (reliable, data);
		bool fast = Protocol.GetMessageType (replyData) != Protocol.MessageType.ROCKET_LAUNCH;
		int coroutineGameCount = gameCount;
		StartCoroutine (Do (delegate {
			if (coroutineGameCount != gameCount) {
				Debug.Log ("***Abandoning async call due to new game");
				return;
			}
			Game.instance.OnRealTimeMessageReceived (reliable, "aiSenderId", replyData);
		}, fast ? 2f : Utils.AI_DELAY));
	}

	IEnumerator Do (Action action, float delay)
	{
		yield return new WaitForSeconds (delay);
		action ();
	}


	byte[] MakeReply (bool reliable, byte[] data)
	{
		switch (Protocol.GetMessageType (data)) {
		case Protocol.MessageType.AIM_AT:
			return MakeAimMessage ();
		case Protocol.MessageType.GRID_POSITIONS:
			return MakeAiGridMessage ();
		case Protocol.MessageType.ROCKET_LAUNCH:
			return MakeAiMoveMessage ();
		default:
			return data;
		}
	}

	byte[] MakeAimMessage ()
	{
		return Protocol.Encode (Protocol.MessageType.AIM_AT, ai.AimRandom (), false);
	}

	byte[] MakeAiGridMessage ()
	{
		var grid = new Grid ();
		grid.SetBoats (Whose.Ours, null);
		return Protocol.Encode (Protocol.MessageType.GRID_POSITIONS, grid, true);
	}

	byte[] MakeAiMoveMessage ()
	{
		Position pos = ai.NextMove ();
		return Protocol.Encode (Protocol.MessageType.ROCKET_LAUNCH, pos, true);
	}

	void SetGameState (GameState gameState)
	{
		this.gameState = gameState;
		OnGameStateChange (gameState);
		if (gameState == GameState.GAME_WAS_TORN_DOWN) {
			SetGameState (GameState.SELECTING_GAME_TYPE);
		}
	}

	public override string ToString ()
	{
		return string.Format ("[{0}: gameState={1}]", name, gameState);
	}
}
