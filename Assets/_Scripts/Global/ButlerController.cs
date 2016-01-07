using UnityEngine;
using UnityEngine.Assertions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.Nearby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using System;

public class ButlerController : MonoBehaviour,RealTimeMultiplayerListener,IDiscoveryListener,IMessageListener
{

	public static ButlerController instance;
	//	public static ButlerController instance { get; private set; }

	public delegate void ConnectStatusAction (bool authenticated, bool isRoomConnected, int roomSetupPercent);

	public event ConnectStatusAction OnConnectStatusChanged;

	//	public IPlayGamesPlatform gamesPlatform { get; private set; }
	IPlayGamesPlatform gamesPlatform;


	bool _authenticated;
	bool _roomConnected;
	int _roomSetupPrecent;

	public bool authenticated {
		get {
			Assert.AreEqual (gamesPlatform.IsAuthenticated (), _authenticated);
			_authenticated = gamesPlatform.IsAuthenticated ();
			return _authenticated;
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

	public List<Participant> GetConnectedParticipants ()
	{
		return gamesPlatform.RealTime.GetConnectedParticipants ();
	}





	public void SetupRoom (bool withInvitation)
	{
		Debug.Log ("***SetupRoom(withInvitation=" + withInvitation + ")");
		Assert.IsTrue (roomSetupPercent == 0);
		if (withInvitation) {
			gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: this);
		} else {
			gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: this);
		}
		roomSetupPercent = 1;
	}

	public ILocalUser GetLocalUser ()
	{
		return gamesPlatform.localUser;
	}


	void InitNearby ()
	{
		Debug.Log ("***Initializing nearby connections …");
		PlayGamesPlatform.InitializeNearby ((client) => {
			Debug.Log ("***Nearby connections initialized: client=" + client);
			StartNearbyBroadcast ();
			StartNearbyDiscovery ();
		});
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		gamesPlatform.RealTime.SendMessageToAll (reliable, data);
	}


	void StartNearbyBroadcast ()
	{
		Debug.Log ("***Nearby broadcast — Starting …");
		List<string> appIdentifiers = new List<string> ();
		//		appIdentifiers.Add (Application.bundleIdentifier);
		appIdentifiers.Add (PlayGamesPlatform.Nearby.GetAppBundleId ());
		PlayGamesPlatform.Nearby.StartAdvertising (
			"VR Battle Grid",
			appIdentifiers,
			TimeSpan.FromSeconds (0),// 0 = advertise forever
			(AdvertisingResult result) => {
				Debug.Log ("***Nearby OnAdvertisingResult() -> Status=" + result.Status + ",Succeeded=" + result.Succeeded + ",LocalEndpointName=" + result.LocalEndpointName);
			},
			(ConnectionRequest request) => {
				Debug.Log ("***Nearby Received connection request: " +
				request.RemoteEndpoint.DeviceId + " " +
				request.RemoteEndpoint.EndpointId + " " +
				request.RemoteEndpoint.Name);
			}
		);
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
			SceneMaster.instance.LoadAsync (SceneMaster.SCENE_GAME);
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

	public void LeaveRoom ()
	{
		Debug.Log ("***LeaveRoom() …");
		gamesPlatform.RealTime.LeaveRoom ();
	}


	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		roomSetupPercent = 0;
		roomConnected = false;
		SceneMaster.instance.LoadAsync (SceneMaster.SCENE_MAIN_MENU);
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
		RealtimeBattle.DecodeAndExecute (data);
	}






	void StartNearbyDiscovery ()
	{
		Debug.Log ("***Nearby discovery — Starting …");
		PlayGamesPlatform.Nearby.StartDiscovery (
			PlayGamesPlatform.Nearby.GetServiceId (),
			TimeSpan.FromSeconds (0),
			(IDiscoveryListener)this);
	}

	//IDiscoveryListener
	public void OnEndpointFound (EndpointDetails discoveredEndpoint)
	{
		Debug.Log ("***Nearby discovery — Found Endpoint: " +
		discoveredEndpoint.DeviceId + " " +
		discoveredEndpoint.EndpointId + " " +
		discoveredEndpoint.Name);
		SendNearbyConnectionRequest (discoveredEndpoint);

	}

	void SendNearbyConnectionRequest (EndpointDetails remote)
	{
		Debug.Log ("***Nearby sending connection request …");
		PlayGamesPlatform.Nearby.SendConnectionRequest (
			"Local Game player",  // the user-friendly name
			remote.EndpointId,  // the discovered endpoint  
			System.Text.Encoding.UTF8.GetBytes ("hello, neighbor"), // byte[] of data
			(response) => {
				Debug.Log ("***Nearby connection response: ResponseStatus" +
				response.ResponseStatus + ",Payload=" + response.Payload);
			},
			(IMessageListener)this);
	}

	//IDiscoveryListener
	public void OnEndpointLost (string lostEndpointId)
	{
		Debug.Log ("***Nearby discovery — Endpoint lost: " + lostEndpointId);
	}

	//IMessageListener
	public void OnMessageReceived (string remoteEndpointId, byte[] data,
	                               bool isReliableMessage)
	{
		Debug.Log ("***Nearby OnMessageReceived() remoteEndpointId=" + remoteEndpointId + ",reliable=" + isReliableMessage + ",data=" + data);
	}

	//IMessageListener
	public void OnRemoteEndpointDisconnected (string remoteEndpointId)
	{
		Debug.Log ("***Nearby OnRemoteEndpointDisconnected() remoteEndpointId=" + remoteEndpointId);
	}

}
