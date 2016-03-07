﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;


public class ButlerPlayGames : BaseButler,RealTimeMultiplayerListener
{
	IPlayGamesPlatform gamesPlatform;

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

	public override void NewGame ()
	{
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
		Debug.Log ("***SignOut() …");
		gamesPlatform.SignOut ();
	}

	void PlayGamesNewGame ()
	{
		Debug.Log ("***PlayGamesNewGame() …");
		Assert.AreEqual (GameState.AUTHENTICATING, gameState);
		Game.instance.SetGameState (GameState.SETTING_UP_GAME);
//		gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : Protocol.PROTOCOL_VERSION, listener: this);
		gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : Protocol.PROTOCOL_VERSION, listener: this);
	}

	public override void QuitGame ()
	{
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


	public override void SendMessageToAll (bool reliable, byte[] data)
	{
		gamesPlatform.RealTime.SendMessageToAll (reliable, data);
	}



	// RealTimeMultiplayerListener
	public void OnRoomSetupProgress (float percent)
	{
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
		Debug.Log ("***OnLeftRoom()");
		CancelCheckup ();
		Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
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
		Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
	}

	// RealTimeMultiplayerListener
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		#if !UNITY_EDITOR
		Debug.Log ("***OnRealTimeMessageReceived(" + isReliable + "," + senderId + ",'" + (char)data [0] + "':" + data.Length + "bytes)");
		#endif
		Game.instance.OnRealTimeMessageReceived (isReliable, senderId, data);
	}

	public override string ToString ()
	{
		return string.Format ("[{0}: gameState={1}]", name, gameState);
	}

}
