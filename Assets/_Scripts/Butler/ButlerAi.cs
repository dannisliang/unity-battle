using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;

public class ButlerAi : MonoBehaviour,IButler
{

	public event Game.GameStateChange OnGameStateChange;

	GameState gameState = GameState.NEED_TO_SELECT_GAME_TYPE;
	GameAi ai;

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
		Destroy (ai);
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		byte[] replyData = MakeReply (reliable, data);
		bool fast = Protocol.GetMessageType (replyData) != Protocol.MessageType.ROCKET_LAUNCH;
		SceneMaster.instance.Async (delegate {
			Game.instance.OnRealTimeMessageReceived (reliable, "aiSenderId", replyData);
		}, fast ? .1f : Utils.AI_DELAY);
	}

	byte[] MakeReply (bool reliable, byte[] data)
	{
		switch (Protocol.GetMessageType (data)) {
		case Protocol.MessageType.GRID_POSITIONS:
			Assert.IsNull (ai);
			ai = gameObject.AddComponent<GameAi> ();
			return MakeAiGridMessage ();
		case Protocol.MessageType.ROCKET_LAUNCH:
			return MakeAiMoveMessage ();
		default:
			return data;
		}
	}

	byte[] MakeAiGridMessage ()
	{
		Grid grid = new Grid ();
		grid.SetBoats (Whose.Theirs, null);
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
			SetGameState (GameState.NEED_TO_SELECT_GAME_TYPE);
		}
	}

	public override string ToString ()
	{
		return string.Format ("[ButlerDemo: gameState={0}]", gameState);
	}
}
