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

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Game : MonoBehaviour,IDiscoveryListener,IMessageListener
{

	public static Game instance;

	public static IButler butler { get; private set; }

	public delegate void ConnectStatusAction (ConnectionStatus status);

	public event ConnectStatusAction OnConnectStatusChanged;

	public delegate void GameTypeChanged (GameType gameType);

	public event GameTypeChanged OnGameTypeChanged;


	GameType _gameType;

	public GameType gameType {
		get {
			return _gameType;
		}
		set {
			_gameType = value;
			InvokeGameTypeChanged ();
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

		if (Application.isEditor) {
			butler = new ButlerDemo ();
		} else {
			butler = new ButlerPlayGames ();
		}
		butler.Init ();
		SignIn (true);
	}

	#if UNITY_EDITOR
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S)) {
			int scale = 2;
			string filename = PlayerSettings.bundleIdentifier + "-" + (Screen.width * scale) + "x" + (Screen.height * scale) + "-" + DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss") + ".png";
			Application.CaptureScreenshot (filename, scale);
			Debug.Log (scale + "x screenshot saved as " + filename);
		}
	}
	#endif

	public void InvokeGameTypeChanged (GameTypeChanged action = null)
	{
		action = action ?? OnGameTypeChanged;
		if (action == null) {
			return;
		}
		action (gameType);
	}

	public void InvokeConnectStatusAction (ConnectStatusAction action = null)
	{
		action = action ?? OnConnectStatusChanged;
		if (action == null) {
			return;
		}
		action (GetConnectionStatus ());
	}

	ConnectionStatus GetConnectionStatus ()
	{
		if (!butler.IsSignedIn ()) {
			return ConnectionStatus.AUTHENTICATION_REQUIRED;
		}
		if (butler.IsGameConnected ()) {
			Assert.IsTrue (butler.GameSetupPercent () == 100);
			return ConnectionStatus.AUTHENTICATED_IN_GAME;
		} else {
			if (butler.GameSetupPercent () == 0) {
				return ConnectionStatus.AUTHENTICATED_NO_GAME;
			} else {
				Debug.Log ("*** TODO: Consider AUTHENTICATED_TEARING_DOWN_GAME case");
				//				return ConnectionStatus.AUTHENTICATED_TEARING_DOWN_GAME;
				return ConnectionStatus.AUTHENTICATED_SETTING_UP_GAME;
			}
		}
	}



	public string GetLocalUsername ()
	{
		return butler.GetLocalUsername ();
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
		butler.SendMessageToAll (reliable, data);
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




	public void SignIn (bool silent = false)
	{
		butler.SignIn (silent);
	}

	public void SignOut ()
	{
		butler.SignOut ();
	}

	public void QuitGame ()
	{
		Debug.Log ("***QuitGame() …");
		butler.QuitGame ();
	}



	public void OnLeftGame ()
	{
		Debug.Log ("***OnLeftGame()");
		SceneMaster.instance.LoadAsync (SceneMaster.SCENE_MAIN_MENU);
	}

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
