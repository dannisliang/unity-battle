using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour,RealTimeMultiplayerListener
{
	public static GameController instance;

	AudioSource source;
	//	bool showingWaitingRoom;

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
		bool IsAuthenticated = PlayGamesPlatform.Instance.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && PlayGamesPlatform.Instance.RealTime.IsRoomConnected ();
		Debug.Log ("***OnApplicationPause(" + pause + "), i.e. " + (pause ? "PAUSED" : "RESUMING") + " [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + "]");
		if (!IsRoomConnected) {
			Debug.Log ("***Workaround Google Play Games bug which doesn't fire the OnLeftRoom() callback by calling it manually …");
			OnLeftRoom ();
		}
	}

	void InitNearby ()
	{
		Debug.Log ("***Initializing nearby connections …");
		PlayGamesPlatform.InitializeNearby ((client) => {
			Debug.Log ("***Nearby connections initialized: client=" + client);
		});
	}

	void Start ()
	{
		InitPlayGamesPlatform ();
		TrySilentAuth ();
	}

	void InitPlayGamesPlatform ()
	{
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
//		PlayGamesPlatform.DebugLogEnabled = true;


		Debug.Log ("***Activating PlayGamesPlatform …");
		PlayGamesPlatform.Activate ();
	}

	void TrySilentAuth ()
	{
		PlayGamesPlatform.Instance.Authenticate ((bool success) => {
			Debug.Log ("***Silent auth attempt was " + (success ? "successful" : "UNSUCCESSFUL"));
		}, true);
	}

	// RealTimeMultiplayerListener
	public void OnRoomSetupProgress (float percent)
	{
		Debug.Log ("***OnRoomSetupProgress(" + percent + ")");
		// show the default waiting room.
//		if (!showingWaitingRoom) {
//			showingWaitingRoom = true;
//			PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI ();
//		}
	}

	// RealTimeMultiplayerListener
	public void OnRoomConnected (bool success)
	{
		Debug.Log ("***OnRoomConnected(" + success + ")");
		if (success) {
			SendOurBoatPositions ();
		}
	}

	void SendOurBoatPositions ()
	{
		IFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
//			formatter.Serialize (stream, new Boat (1));
			BattleshipController.instance.boatsOursPlacementController.RecreateBoats ();
			formatter.Serialize (stream, BattleshipController.instance.boatsOursPlacementController.boats);
			byte[] bytes = stream.ToArray ();
			//PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, bytes);
		}
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		SceneManager.LoadScene ("__MainMenu");
//		boatPlacementController.DestroyBoats ();
	}

	// RealTimeMultiplayerListener
	public void OnParticipantLeft (Participant participant)
	{
		Debug.Log ("***OnParticipantLeft(" + participant + ")");
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
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
	}

	// RealTimeMultiplayerListener
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		Debug.Log ("***OnRealTimeMessageReceived(" + isReliable + "," + senderId + "," + data + ")");
	}

}
