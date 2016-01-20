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

	GameState gameState = GameState.SELECTING_GAME_TYPE;

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
		if (gamesPlatform == null) {
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
		Debug.Log ("***Checkup()");
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		if (!IsRoomConnected && (gameState == GameState.PLAYING || gameState == GameState.SETTING_UP_GAME)) {
			Debug.Log ("************************************************\n***Checkup() [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", gameState=" + gameState + "]");
			CancelCheckup ();
			WorkaroundPlayGamePauseBug ();
		}
	}

	void CancelCheckup ()
	{
		Debug.Log ("***CancelInvoke(Checkup)");
		CancelInvoke ("Checkup");
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
		if (gamesPlatform != null) {
			return;
		}
		Debug.Log ("***Init() …");
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

			Debug.Log ("***PlayGamesPlatform.InitializeInstance() …");
			PlayGamesPlatform.InitializeInstance (config);

			// recommended for debugging:
			//			PlayGamesPlatform.DebugLogEnabled = true;

			Debug.Log ("***PlayGamesPlatform.Activate() …");
			PlayGamesPlatform.Activate ();
			gamesPlatform = PlayGamesPlatform.Instance;
		}
	}

	public void NewGame ()
	{
		Debug.Log ("***NewGame() …");
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
		Debug.Log ("***PlayGamesSignIn() …");
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, gameState);
		SetGameState (GameState.AUTHENTICATING);
//		// check if already signed in
//		if (gamesPlatform.IsAuthenticated ()) {
//			callback (true);
//		} else {
		gamesPlatform.Authenticate (callback, false);
//		}
	}

	void PlayGamesSignOut ()
	{
		Debug.Log ("***SignOut() …");
		gamesPlatform.SignOut ();
	}

	void PlayGamesNewGame ()
	{
		Debug.Log ("***PlayGamesNewGame() …");
		Assert.AreEqual (GameState.AUTHENTICATING, gameState);
		SetGameState (GameState.SETTING_UP_GAME);
//		gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : Protocol.PROTOCOL_VERSION, listener: this);
		gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : Protocol.PROTOCOL_VERSION, listener: this);
	}

	public void StartGamePlay ()
	{
		Assert.AreEqual (GameState.SELECTING_VIEW_MODE, gameState);
		SetGameState (GameState.PLAYING);
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
		Debug.Log ("***QuitGame() …");
		switch (gameState) {
		case GameState.AUTHENTICATING:
			PlayGamesSignOut ();
			break;
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.SELECTING_VIEW_MODE:
		case GameState.PLAYING:
			PlayGamesLeaveRoom ();
			break;
		case GameState.GAME_WAS_TORN_DOWN:
		case GameState.SELECTING_GAME_TYPE:
			break;
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
			SetGameState (GameState.SELECTING_VIEW_MODE);
		} else {
			SetGameState (GameState.SETTING_UP_GAME);
		}
	}

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		Debug.Log ("***OnRoomConnected(" + success + ")");
		SetGameState (success ? GameState.SELECTING_VIEW_MODE : GameState.GAME_WAS_TORN_DOWN);
		if (success) {
			Debug.Log ("***InvokeRepeating(Checkup, 1, 1)");
			InvokeRepeating ("Checkup", 1f, 1f);
		}
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		CancelCheckup ();
		SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
		Debug.Log ("***OnParticipantLeft: Mandatory call to LeaveRoom () in order to cleanup …");
		PlayGamesLeaveRoom ();
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
		Debug.Log ("***OnPeersDisconnected: Mandatory call to LeaveRoom () in order to cleanup …");
		PlayGamesLeaveRoom ();
	}

	void PlayGamesLeaveRoom ()
	{
		Debug.Log ("***PlayGamesLeaveRoom()");
		CancelCheckup ();
		if (gamesPlatform.RealTime != null) {
			gamesPlatform.RealTime.LeaveRoom ();
		}
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
			SetGameState (GameState.SELECTING_GAME_TYPE);
		}
	}

	public override string ToString ()
	{
		return string.Format ("[{0}: gameState={1}]", name, gameState);
	}

}
