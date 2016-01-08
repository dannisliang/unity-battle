using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

public class ButlerPlayGames : MonoBehaviour,IButler,RealTimeMultiplayerListener
{
	IPlayGamesPlatform gamesPlatform;

	//	public event Game.ConnectStatusAction OnConnectChanged;

	bool _signedIn;
	bool _gameConnected;
	int _gameSetupPercent;


	public bool IsSignedIn ()
	{
		return signedIn;
	}

	public bool IsGameConnected ()
	{
		return gameConnected;
	}

	public int GameSetupPercent ()
	{
		return gameSetupPercent;
	}

	public bool signedIn {
		get {
			Assert.AreEqual (gamesPlatform.IsAuthenticated (), _signedIn);
			_signedIn = gamesPlatform.IsAuthenticated ();
			return _signedIn;
		}
		set {
			_signedIn = value;
			Game.instance.InvokeConnectStatusAction ();
		}
	}

	public bool gameConnected {
		get {
			Assert.AreEqual (gamesPlatform.IsAuthenticated () && gamesPlatform.RealTime.IsRoomConnected (), _gameConnected);
			return _gameConnected;
		}
		set {
			_gameConnected = value;
			Game.instance.InvokeConnectStatusAction ();
		}
	}

	public int gameSetupPercent {
		get {
			return _gameSetupPercent;
		}
		set { 
			_gameSetupPercent = value;
			Game.instance.InvokeConnectStatusAction ();
		}
	}

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
		Debug.Log ("---------------------------------------\n***Application " + (pause ? "PAUSED" : "RESUMING") + " OnApplicationPause(" + pause + ") [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", roomSetupPercent=" + gameSetupPercent + "]");
		//		if (!pause && roomSetupPercent > 0 && !IsRoomConnected) {
		//			WorkaroundPlayGamePauseBug();
		//		}
	}

	void Checkup ()
	{
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		if (!IsRoomConnected && gameSetupPercent == 100) {
			Debug.Log ("************************************************\n***Checkup() [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", roomSetupPercent=" + gameSetupPercent + "]");
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


	public void SignIn (bool silent = false)
	{
		Debug.Log ("***SignIn(" + (silent ? "silent" : "loud") + ") …");
		gamesPlatform.Authenticate ((bool success) => {
			Debug.Log ("***Auth attempt was " + (success ? "successful" : "UNSUCCESSFUL"));
			signedIn = success;
		}, silent);
	}

	public void SignOut ()
	{
		Debug.Log ("***SignOut() …");
		gamesPlatform.SignOut ();
		Game.instance.InvokeConnectStatusAction ();
	}



	public void SetupGame (bool withInvitation)
	{
		Debug.Log ("***SetupGame(withInvitation=" + withInvitation + ")");
		Assert.IsTrue (gameSetupPercent == 0);
		if (withInvitation) {
			gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: this);
		} else {
			gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: this);
		}
		gameSetupPercent = 1;
	}

	public void QuitGame ()
	{
		Debug.Log ("***QuitGame() …");
		gamesPlatform.RealTime.LeaveRoom ();
	}


	public void SendMessageToAll (bool reliable, byte[] data)
	{
		gamesPlatform.RealTime.SendMessageToAll (reliable, data);
	}



	// RealTimeMultiplayerListener
	public void OnRoomSetupProgress (float percent)
	{
		Debug.Log ("***OnRoomSetupProgress(" + percent + ")");
		gameSetupPercent = (int)percent;
		// show the default waiting room.
		//		if (!showingWaitingRoom) {
		//			showingWaitingRoom = true;
		//			gamesPlatform.RealTime.ShowWaitingRoomUI ();
		//		}
	}

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		Debug.Log ("***OnRoomConnected(" + success + ")");
		gameSetupPercent = success ? 100 : 0;
		gameConnected = success;
		if (success) {
			SceneMaster.instance.LoadAsync (SceneMaster.SCENE_GAME);
			InvokeRepeating ("Checkup", 1f, 1f);
		}
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		gameSetupPercent = 0;
		gameConnected = false;
		Game.instance.OnLeftGame ();
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
		QuitGame ();
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
		QuitGame ();
	}

	// RealTimeMultiplayerListener
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		Debug.Log ("***OnRealTimeMessageReceived(" + isReliable + "," + senderId + "," + (char)data [0] + "-" + data.Length + ")");
		Game.instance.OnRealTimeMessageReceived (isReliable, senderId, data);
	}



}
