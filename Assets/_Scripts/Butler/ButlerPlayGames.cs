using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;


public class ButlerPlayGames : BaseButler,RealTimeMultiplayerListener
{
	static string CATEGORY = typeof(ButlerPlayGames).Name;

	GoogleAnalyticsV4 gav4;
	IPlayGamesPlatform gamesPlatform;

	void Awake ()
	{
		gav4 = AnalyticsAssistant.gav4;
	}

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

	public override int NumPlayers ()
	{
		return gamesPlatform.RealTime.GetConnectedParticipants ().Count;
	}

	public override string GetLocalUsername ()
	{
		return gamesPlatform.localUser.userName;
	}

	void OnApplicationPause (bool pause)
	{
		gav4.LogEvent (CATEGORY, "OnApplicationPause", Convert.ToString (pause), 0);
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
//		Debug.Log ("***Checkup()");
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		if (!IsRoomConnected && (gameState == GameState.SELECTING_VIEW_MODE || gameState == GameState.PLAYING || gameState == GameState.SETTING_UP_GAME)) {
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
		gav4.LogEvent (CATEGORY, "WorkaroundPlayGamePauseBug", null, 0);
		Debug.Log ("***Workaround: Google Play Games bug which doesn't fire the OnLeftRoom() callback by calling LeaveRoom() / OnLeftRoom() manually …");
		if (gamesPlatform.RealTime != null) {
			Debug.Log ("***Workaround: Calling LeaveRoom() …");
			QuitGame ();
		}
		Debug.Log ("***Workaround: Calling OnLeftRoom() …");
		OnLeftRoom ();
	}



	override protected void OnEnable ()
	{
		gav4.LogEvent (CATEGORY, "OnEnable", null, 0);
		base.OnEnable ();

		if (gamesPlatform != null) {
			return;
		}
		Debug.Log ("***Initialize Play Games …");
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

	override protected void OnDisable ()
	{
		gav4.LogEvent (CATEGORY, "OnDisable", null, 0);
		base.OnDisable ();
	}

	public override void NewGame ()
	{
		gav4.LogEvent (CATEGORY, "NewGame", null, 0);
		Debug.Log ("***NewGame()");
		#if !UNITY_EDITOR
		CheckInternetReachability ();
		#endif
		PlayGamesSignIn ((bool success) => {
			Debug.Log ("***Auth attempt was " + (success ? "successful" : "UNSUCCESSFUL"));
			if (success) {
				PlayGamesNewGame ();
			} else {
				Game.instance.SetErrorFailureReasonText ("— Google Play Games failed to Sign In —");
				Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
			}
		});
	}

	void PlayGamesSignIn (Action<bool> callback)
	{
		gav4.LogEvent (CATEGORY, "PlayGamesSignIn", null, 0);
		Debug.Log ("***PlayGamesSignIn() …");
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, gameState);
		Game.instance.SetGameState (GameState.AUTHENTICATING);
//		// check if already signed in
//		if (gamesPlatform.IsAuthenticated ()) {
//			callback (true);
//		} else {
		gamesPlatform.Authenticate (callback, false);
//		}
	}

	void PlayGamesSignOut ()
	{
		gav4.LogEvent (CATEGORY, "PlayGamesSignOut", null, 0);
		Debug.Log ("***SignOut() …");
		gamesPlatform.SignOut ();
	}

	void PlayGamesNewGame ()
	{
		gav4.LogEvent (CATEGORY, "PlayGamesNewGame", null, 0);
		Debug.Log ("***PlayGamesNewGame() …");
		Assert.AreEqual (GameState.AUTHENTICATING, gameState);
		Game.instance.SetGameState (GameState.SETTING_UP_GAME);
//		gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : Protocol.PROTOCOL_VERSION, listener: this);
		gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : Protocol.PROTOCOL_VERSION, listener: this);
	}

	public override void QuitGame ()
	{
		gav4.LogEvent (CATEGORY, "QuitGame", Convert.ToString (gameState), 0);
		Debug.Log ("***QuitGame() …");
		switch (gameState) {
		case GameState.AUTHENTICATING:
			PlayGamesSignOut ();
			PlayGamesLeaveRoom ();
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


	public override void SendMessageToAll (bool reliable, ref byte[] data)
	{
		gamesPlatform.RealTime.SendMessageToAll (reliable, data);
	}



	// RealTimeMultiplayerListener
	public void OnRoomSetupProgress (float percent)
	{
		gav4.LogEvent (CATEGORY, "OnRoomSetupProgress", Convert.ToString (percent), 0);
		Debug.Log ("***OnRoomSetupProgress(" + percent + ")");
		if (percent == 0) {
			Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
		} else if (percent == 100) {
			Game.instance.SetGameState (GameState.SELECTING_VIEW_MODE);
		}
	}

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		gav4.LogEvent (CATEGORY, "OnRoomConnected", Convert.ToString (success), 0);
		Debug.Log ("***OnRoomConnected(" + success + ")");
		Game.instance.SetGameState (success ? GameState.SELECTING_VIEW_MODE : GameState.GAME_WAS_TORN_DOWN);
		if (success) {
			Debug.Log ("***InvokeRepeating(Checkup, 3, 3)");
			InvokeRepeating ("Checkup", 3f, 3f);
		}
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		gav4.LogEvent (CATEGORY, "OnLeftRoom", null, 0);
		Debug.Log ("***OnLeftRoom()");
		CancelCheckup ();
		Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		gav4.LogEvent (CATEGORY, "OnParticipantLeft", null, 0);
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
		Debug.Log ("***OnParticipantLeft: Mandatory call to LeaveRoom () in order to cleanup …");
		PlayGamesLeaveRoom ();
	}

	// RealTimeMultiplayerListener
	public void OnPeersConnected (string[] participantIds)
	{
		gav4.LogEvent (CATEGORY, "OnPeersConnected", null, participantIds.Length);
		Debug.Log ("***OnPeersConnected(" + string.Join (",", participantIds) + ")");
	}

	// RealTimeMultiplayerListener
	public void OnPeersDisconnected (string[] participantIds)
	{
		gav4.LogEvent (CATEGORY, "OnPeersDisconnected", null, participantIds.Length);
		Debug.Log ("***OnPeersDisconnected(" + string.Join (",", participantIds) + ")");
		Debug.Log ("***OnPeersDisconnected: Mandatory call to LeaveRoom () in order to cleanup …");
		PlayGamesLeaveRoom ();
	}

	void PlayGamesLeaveRoom ()
	{
		gav4.LogEvent (CATEGORY, "PlayGamesLeaveRoom", null, 0);
		Debug.Log ("***PlayGamesLeaveRoom()");
		CancelCheckup ();
		if (gamesPlatform.RealTime != null) {
			gamesPlatform.RealTime.LeaveRoom ();
		}
		Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		#if !UNITY_EDITOR
		Debug.Log ("***OnRealTimeMessageReceived(" + isReliable + "," + senderId + ",'" + (char)data [0] + "':" + data.Length + "bytes)");
		#endif
		Game.instance.OnRealTimeMessageReceived (isReliable, senderId, ref data);
	}

	public override string ToString ()
	{
		return string.Format ("[{0}: gameState={1}]", name, gameState);
	}

}
