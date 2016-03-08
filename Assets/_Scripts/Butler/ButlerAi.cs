using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;
using System;

public class ButlerAi : BaseButler
{
	static int gameCount;

	GameAi ai;

	//	Whose whoseTurn;
	//	bool firing;
	Whose loser;

	override protected void OnEnable ()
	{
		base.OnEnable ();

		BattleController.instance.OnBattleState += UpdateBattleState;
	}

	override protected void OnDisable ()
	{
		base.OnDisable ();

		BattleController.instance.OnBattleState -= UpdateBattleState;
	}

	void UpdateBattleState (Whose whoseTurn, bool firing, Whose loser)
	{
//		this.whoseTurn = whoseTurn;
//		this.firing = firing;
		this.loser = loser;
	}

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

	public override int NumPlayers ()
	{
		return (gameState == GameState.SELECTING_VIEW_MODE || gameState == GameState.PLAYING) ? 2 : 0;
	}

	public override string GetLocalUsername ()
	{
		return (gameState == GameState.SELECTING_VIEW_MODE || gameState == GameState.PLAYING) ? "Ford Prefect" : "";
	}

	public override void NewGame ()
	{
		gameCount++;
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, gameState);
		Game.instance.SetGameState (GameState.AUTHENTICATING);
		Game.instance.SetGameState (GameState.SETTING_UP_GAME);
		Game.instance.SetGameState (GameState.SELECTING_VIEW_MODE);
		if (ai != null) {
			Destroy (ai);
		}
		ai = gameObject.AddComponent<GameAi> ();
//		StartCoroutine (KeepOnAiming ());
	}

	public override void QuitGame ()
	{
		StopAllCoroutines ();
		if (gameState == GameState.PLAYING || gameState == GameState.SELECTING_VIEW_MODE) {
			Game.instance.SetGameState (GameState.TEARING_DOWN_GAME);
		}
		if (gameState == GameState.TEARING_DOWN_GAME) {
			Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
			Destroy (ai);
		}
	}

	//	IEnumerator KeepOnAiming ()
	//	{
	//		while (true) {
	//			if (whoseTurn == Whose.Theirs) {
	//				SendNow (false, MakeAimMessage ());
	//			}
	//			yield return new WaitForSeconds (.8f);
	//		}
	//	}

	public override void SendMessageToAll (bool reliable, ref byte[] data)
	{
		if (!enabled) {
			Debug.Log ("***INGORING SendMessageToAll() as " + name + " is disabled");
			return;
		}
		if (loser != Whose.Nobody) {
			Debug.Log ("***INGORING SendMessageToAll() due to loser=" + loser);
			return;
		}
		switch (Protocol.GetMessageType (ref data)) {
		case Protocol.MessageType.GRID_POSITIONS:
			byte[] gridMessage = MakeAiGridMessage ();
			DelayedSend (reliable, ref gridMessage);
			break;
		case Protocol.MessageType.ROCKET_LAUNCH:
			byte[] launchMessage = MakeAiLaunchMessage ();
			DelayedSend (reliable, ref launchMessage);
			break;
		default:
			break;
		}
	}

	void DelayedSend (bool reliable, ref byte[] data)
	{
		byte[] clone = (byte[])data.Clone ();
		int coroutineGameCount = gameCount;
		StartCoroutine (Do (delegate {
			if (loser != Whose.Nobody) {
				Debug.Log ("***Abandoning async call due to loser=" + loser);
				return;
			}
			if (coroutineGameCount != gameCount) {
				Debug.Log ("***Abandoning async call due to new game");
				return;
			}
			SendNow (reliable, ref clone);
		}, GetMessageDelay (Protocol.GetMessageType (ref data))));
	}

	public static float GetMessageDelay (Protocol.MessageType messageType)
	{
		switch (messageType) {
		case Protocol.MessageType.AIM_AT:
			return 2f;
		case Protocol.MessageType.GRID_POSITIONS:
			return .2f;
		case Protocol.MessageType.ROCKET_LAUNCH:
			return 7f;
		default:
			throw new NotImplementedException ();
		}
	}

	void SendNow (bool reliable, ref byte[] data)
	{
		Game.instance.OnRealTimeMessageReceived (reliable, "aiSenderId", ref data);
	}

	IEnumerator Do (Action action, float delay)
	{
		yield return new WaitForSeconds (delay);
		action ();
	}

	byte[] MakeAimMessage ()
	{
		return Protocol.Encode (Protocol.MessageType.AIM_AT, ai.AimRandom (), false);
	}

	public static byte[] MakeAiGridMessage ()
	{
		var grid = new Grid (Utils.AI_PLAYER_ID);
		grid.SetBoats (Whose.Ours, null);
		return Protocol.Encode (Protocol.MessageType.GRID_POSITIONS, grid, true);
	}

	byte[] MakeAiLaunchMessage ()
	{
		Position pos = ai.NextMove ();
		return Protocol.Encode (Protocol.MessageType.ROCKET_LAUNCH, pos, true);
	}

	public override string ToString ()
	{
		return string.Format ("[{0}: gameState={1}]", name, gameState);
	}
}
