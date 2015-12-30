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

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour,RealTimeMultiplayerListener
{
	public static GameController instance;

	public static IPlayGamesPlatform gamesPlatform { get; private set; }

	AudioSource source;
	//	bool showingWaitingRoom;
	int roomSetupPercent;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);

		SceneManager.LoadScene ("__MainMenu");
//		InitNearby ();
	}

	void OnApplicationPause (bool pause)
	{
		if (Time.frameCount <= 1) {
			return;
		}
		bool IsAuthenticated = gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && gamesPlatform.RealTime.IsRoomConnected ();
		Debug.Log ("***OnApplicationPause(" + pause + "), i.e. " + (pause ? "PAUSED" : "RESUMING") + " [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + "]");
		if (!IsRoomConnected) {
			Debug.Log ("***Workaround Google Play Games bug which doesn't fire the OnLeftRoom() callback by calling it manually …");
			OnLeftRoom ();
		}
	}

	//	void InitNearby ()
	//	{
	//		Debug.Log ("***Initializing nearby connections …");
	//		PlayGamesPlatform.InitializeNearby ((client) => {
	//			Debug.Log ("***Nearby connections initialized: client=" + client);
	//		});
	//	}

	void Start ()
	{
		InitPlayGamesPlatform ();
		Authenticate (true);
	}

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
		}, silent);
	}

	public int RoomSetupPercent ()
	{
		return roomSetupPercent;
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

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		Debug.Log ("***OnRoomConnected(" + success + ")");
		roomSetupPercent = success ? 100 : 0;
		if (success) {
			SceneManager.LoadScene ("__BattleshipGame");
		}
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
		SceneManager.LoadScene ("__MainMenu");
//		boatPlacementController.DestroyBoats ();
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
		gamesPlatform.RealTime.LeaveRoom ();
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
		gamesPlatform.RealTime.LeaveRoom ();
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
			GameController.gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: this);
		} else {
			GameController.gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: this);
		}
		roomSetupPercent = 1;
	}

}
