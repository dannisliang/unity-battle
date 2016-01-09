using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;


public class ButlerPlayGames : MonoBehaviour,IButler,RealTimeMultiplayerListener
{
	IPlayGamesPlatform gamesPlatform;

	public event Game.GameStateChange OnGameStateChange;

	GameState gameState = GameState.NEED_TO_SELECT_GAME_TYPE;

	#if UNITY_EDITOR
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F) && gamesPlatform.IsAuthenticated ()) {
			Debug.Log ("***Simulating failure …");
			if (gamesPlatform.RealTime.IsRoomConnected ()) {
				PlayGamesSignOut ();
			} else {
				QuitGame ();
			}
		}
	}
	#endif

	public int NumPlayers ()
	{
		return gamesPlatform.RealTime.GetConnectedParticipants ().Count;
	}

	public string GetLocalUsername ()
	{
		return gamesPlatform.localUser.userName;
	}

	void OnApplicationPause (bool pause)
	{
		if (Time.frameCount <= 1) {
			return;
		}
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		Debug.Log ("---------------------------------------\n***Application " + (pause ? "PAUSED" : "RESUMING") + " OnApplicationPause(" + pause + ") [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", gameState=" + gameState + "]");
		//		if (!pause && roomSetupPercent > 0 && !IsRoomConnected) {
		//			WorkaroundPlayGamePauseBug();
		//		}
	}

	void Checkup ()
	{
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		if (!IsRoomConnected && gameState == GameState.PLAYING) {
			Debug.Log ("************************************************\n***Checkup() [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", gameState=" + gameState + "]");
			StopCoroutine ("Checkup");
			WorkaroundPlayGamePauseBug ();
		}
	}

	void WorkaroundPlayGamePauseBug ()
	{
		Debug.Log ("***Workaround: Google Play Games bug which doesn't fire the OnLeftRoom() callback by calling LeaveRoom() / OnLeftRoom() manually …");
		if (gamesPlatform.RealTime != null) {
			Debug.Log ("***Workaround: Calling LeaveRoom() …");
			QuitGame ();
		}
		Debug.Log ("***Workaround: Calling OnLeftRoom() …");
		OnLeftRoom ();
	}



	public void Init ()
	{
		if (Application.isEditor) {
			gamesPlatform = new DummyPlayGamesPlatform ();
		} else {
			// https://github.com/playgameservices/play-games-plugin-for-unity
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ()
			                                      // enables saving game progress.
			                                      //.EnableSavedGames ()
			                                      // registers a callback to handle game invitations received while the game is not running.
			                                      //.WithInvitationDelegate(<callback method>)
			                                      // registers a callback for turn based match notifications received while the
			                                      // game is not running.
			                                      //.WithMatchDelegate(<callback method>)
				.Build ();

			PlayGamesPlatform.InitializeInstance (config);

			// recommended for debugging:
			//			PlayGamesPlatform.DebugLogEnabled = true;

			Debug.Log ("***Activating PlayGamesPlatform …");
			PlayGamesPlatform.Activate ();
			gamesPlatform = PlayGamesPlatform.Instance;
		}
	}

	public void NewGame ()
	{
		PlayGamesSignIn ((bool success) => {
			Debug.Log ("***Auth attempt was " + (success ? "successful" : "UNSUCCESSFUL"));
			if (success) {
				PlayGamesNewGame ();
			} else {
				SetGameState (GameState.GAME_WAS_TORN_DOWN);
			}
		});
	}

	void PlayGamesSignIn (Action<bool> callback)
	{
		Assert.AreEqual (GameState.NEED_TO_SELECT_GAME_TYPE, gameState);
		SetGameState (GameState.AUTHENTICATING);
		// check if already signed in
		if (gamesPlatform.IsAuthenticated ()) {
			callback (true);
		} else {
			gamesPlatform.Authenticate (callback, false);
		}
	}

	void PlayGamesSignOut ()
	{
		Debug.Log ("***SignOut() …");
		gamesPlatform.SignOut ();
	}

	void PlayGamesNewGame ()
	{
		Assert.AreEqual (GameState.AUTHENTICATING, gameState);
		SetGameState (GameState.SETTING_UP_GAME);
//		gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: this);
		gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: this);
	}

	public GameState GetGameState ()
	{
		return gameState;
	}

	public void QuitGame ()
	{
		Debug.Log ("***QuitGame() …");
		switch (gameState) {
		case GameState.PLAYING:
			gamesPlatform.RealTime.LeaveRoom ();
			break;
		case GameState.NEED_TO_SELECT_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.GAME_WAS_TORN_DOWN:
		default:
			throw new NotImplementedException ();
		}

	}


	public void SendMessageToAll (bool reliable, byte[] data)
	{
		gamesPlatform.RealTime.SendMessageToAll (reliable, data);
	}



	// RealTimeMultiplayerListener
	public void OnRoomSetupProgress (float percent)
	{
		Debug.Log ("***OnRoomSetupProgress(" + percent + ")");
		if (percent == 0) {
			SetGameState (GameState.GAME_WAS_TORN_DOWN);
		} else if (percent == 100) {
			SetGameState (GameState.PLAYING);
		} else {
			SetGameState (GameState.SETTING_UP_GAME);
		}
	}

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		Debug.Log ("***OnRoomConnected(" + success + ")");
		SetGameState (success ? GameState.PLAYING : GameState.GAME_WAS_TORN_DOWN);
		if (success) {
			InvokeRepeating ("Checkup", 1f, 1f);
		}
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
		SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnPeersConnected (string[] participantIds)
	{
		Debug.Log ("***OnPeersConnected(" + string.Join (",", participantIds) + ")");
	}

	// RealTimeMultiplayerListener
	public void OnPeersDisconnected (string[] participantIds)
	{
		Debug.Log ("***OnPeersDisconnected(" + string.Join (",", participantIds) + ")");
		SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		Debug.Log ("***OnRealTimeMessageReceived(" + isReliable + "," + senderId + "," + (char)data [0] + "-" + data.Length + ")");
		Game.instance.OnRealTimeMessageReceived (isReliable, senderId, data);
	}

	void SetGameState (GameState state)
	{
		gameState = state;
		OnGameStateChange (gameState);
		if (gameState == GameState.GAME_WAS_TORN_DOWN) {
			SetGameState (GameState.NEED_TO_SELECT_GAME_TYPE);
		}
	}

	public override string ToString ()
	{
		return string.Format ("[ButlerPlayGames: gameState={0}]", gameState);
	}

}
