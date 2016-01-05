using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour,RealTimeMultiplayerListener
{
	public static GameController instance { get; private set; }

	IPlayGamesPlatform gamesPlatform;

	public static event ConnectStatusAction OnConnectStatusChanged;

	public delegate void ConnectStatusAction (bool authenticated, bool isRoomConnected, int roomSetupPercent);

	public bool quitting { get; private set; }

	bool _authenticated;
	bool _roomConnected;
	int _roomSetupPrecent;

	AudioSource source;
	//	bool showingWaitingRoom;

	public bool authenticated {
		get {
			Assert.AreEqual (gamesPlatform.IsAuthenticated (), _authenticated);
			return gamesPlatform.IsAuthenticated ();
		}
		set {
			_authenticated = value;
			InvokeConnectStatusAction ();
		}
	}

	public bool roomConnected {
		get {
			Assert.AreEqual (gamesPlatform.IsAuthenticated () && gamesPlatform.RealTime.IsRoomConnected (), _roomConnected);
			return _roomConnected;
		}
		set {
			_roomConnected = value;
			InvokeConnectStatusAction ();
		}
	}

	public int roomSetupPercent {
		get {
			return _roomSetupPrecent;
		}
		set { 
			_roomSetupPrecent = value;
			InvokeConnectStatusAction ();
		}
	}

	public void InvokeConnectStatusAction (ConnectStatusAction action = null)
	{
		action = action ?? OnConnectStatusChanged;
		if (action == null) {
			return;
		}
		bool authenticated = gamesPlatform.IsAuthenticated ();
		bool roomConnected = authenticated && gamesPlatform.RealTime.IsRoomConnected ();
		action (gamesPlatform.IsAuthenticated (), roomConnected, roomSetupPercent);
	}

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);

//		InitNearby ();

		InitPlayGamesPlatform ();
		Authenticate (true);
		Debug.Log ("***Loading " + Utils.SCENE_MAIN_MENU + " …");
		SceneManager.LoadScene (Utils.SCENE_MAIN_MENU);
	}

	void OnApplicationQuit ()
	{
		quitting = true;
	}

	//	void InitNearby ()
	//	{
	//		Debug.Log ("***Initializing nearby connections …");
	//		PlayGamesPlatform.InitializeNearby ((client) => {
	//			Debug.Log ("***Nearby connections initialized: client=" + client);
	//		});
	//	}

	void InitPlayGamesPlatform ()
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

	public void Authenticate (bool silent)
	{
		Debug.Log ("***Authenticate(" + (silent ? "silent" : "loud") + ") …");
		gamesPlatform.Authenticate ((bool success) => {
			Debug.Log ("***Auth attempt was " + (success ? "successful" : "UNSUCCESSFUL"));
			authenticated = success;
		}, silent);
	}

	public void SignOut ()
	{
		Debug.Log ("***SignOut() …");
		gamesPlatform.SignOut ();
		InvokeConnectStatusAction ();
	}

	// RealTimeMultiplayerListener
	public void OnRoomSetupProgress (float percent)
	{
		Debug.Log ("***OnRoomSetupProgress(" + percent + ")");
		roomSetupPercent = (int)percent;
		// show the default waiting room.
//		if (!showingWaitingRoom) {
//			showingWaitingRoom = true;
//			gamesPlatform.RealTime.ShowWaitingRoomUI ();
//		}
	}

	void OnApplicationPause (bool pause)
	{
		if (Time.frameCount <= 1) {
			return;
		}
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		Debug.Log ("---------------------------------------\n***Application " + (pause ? "PAUSED" : "RESUMING") + " OnApplicationPause(" + pause + ") [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", roomSetupPercent=" + roomSetupPercent + "]");
		//		if (!pause && roomSetupPercent > 0 && !IsRoomConnected) {
		//			WorkaroundPlayGamePauseBug();
		//		}
	}

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		Debug.Log ("***OnRoomConnected(" + success + ")");
		roomSetupPercent = success ? 100 : 0;
		roomConnected = success;
		if (success) {
			Debug.Log ("***Loading " + Utils.SCENE_BATTLESHIP_GAME + " …");
			SceneManager.LoadScene (Utils.SCENE_BATTLESHIP_GAME);
			InvokeRepeating ("Checkup", 1f, 1f);
		}
	}

	void Checkup ()
	{
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		if (!IsRoomConnected && roomSetupPercent == 100) {
			Debug.Log ("************************************************\n***Checkup() [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", roomSetupPercent=" + roomSetupPercent + "]");
			WorkaroundPlayGamePauseBug ();
		}
	}

	void WorkaroundPlayGamePauseBug ()
	{
		Debug.Log ("***Workaround: Google Play Games bug which doesn't fire the OnLeftRoom() callback by calling LeaveRoom() / OnLeftRoom() manually …");
		if (gamesPlatform.RealTime != null) {
			Debug.Log ("***Workaround: Calling LeaveRoom() …");
			LeaveRoom ();
		}
		Debug.Log ("***Workaround: Calling OnLeftRoom() …");
		OnLeftRoom ();
	}

	public ILocalUser GetLocalUser ()
	{
		return gamesPlatform.localUser;
	}

	public List<Participant> GetConnectedParticipants ()
	{
		return gamesPlatform.RealTime.GetConnectedParticipants ();
	}

	public void LeaveRoom ()
	{
		Debug.Log ("***LeaveRoom() …");
		gamesPlatform.RealTime.LeaveRoom ();
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		gamesPlatform.RealTime.SendMessageToAll (reliable, data);
	}

	public void SendOurBoatPositions ()
	{
		RealtimeBattleship.EncodeAndSend (BattleshipController.instance.boatsOursPlacementController.grid);
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		roomSetupPercent = 0;
		roomConnected = false;
		Debug.Log ("***Loading " + Utils.SCENE_MAIN_MENU + " …");
		SceneManager.LoadScene (Utils.SCENE_MAIN_MENU);
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
		LeaveRoom ();
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
		LeaveRoom ();
	}

	// RealTimeMultiplayerListener
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		Debug.Log ("***OnRealTimeMessageReceived(" + isReliable + "," + senderId + "," + (char)data [0] + "-" + data.Length + ")");
		RealtimeBattleship.DecodeAndExecute (data);
	}

	public void ExecuteDelayed (Action action, float delay)
	{
		StartCoroutine (ExecuteDelayedCoroutine (action, delay));
	}

	IEnumerator ExecuteDelayedCoroutine (Action action, float delay)
	{
		yield return new  WaitForSeconds (delay);
		action ();
	}

	public void SetupRoom (bool withInvitation)
	{
		Assert.IsTrue (roomSetupPercent == 0);
		if (withInvitation) {
			gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: this);
		} else {
			gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: this);
		}
		roomSetupPercent = 1;
	}

}
