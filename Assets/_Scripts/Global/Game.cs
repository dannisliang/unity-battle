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

	public delegate void GameStateChange (GameState state);

	object GameStateChangeLock = new System.Object ();

	private event GameStateChange _OnGameStateChange;

	public event GameStateChange OnGameStateChange {
		add {
			lock (GameStateChangeLock) {
				_OnGameStateChange += value;
				value (butler == null ? GameState.SELECTING_GAME_TYPE : butler.GetGameState ());
			}
		}
		remove {
			lock (GameStateChangeLock) {
				_OnGameStateChange -= value;
			}
		}
	}

	public delegate void GameTypeChanged (GameType gameType);

	public event GameTypeChanged OnGameTypeChanged;


	GameType _gameType;

	public GameType gameType {
		get {
			return _gameType;
		}
		set {
			_gameType = value;
			if (OnGameTypeChanged != null) {
				OnGameTypeChanged (_gameType);
			}
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

		gameObject.AddComponent<ButlerDemo> ();
		gameObject.AddComponent<ButlerPlayGames> ();

		OnGameTypeChanged += HandleGameTypeChanged;
		OnGameStateChange += HandleGameStateChanged;
	}

	void HandleGameTypeChanged (GameType gameType)
	{
		butler = GetButler (gameType);
		butler.OnGameStateChange += (GameState state) => {
			if (_OnGameStateChange != null) {
				_OnGameStateChange (state);
			}
		};
		butler.Init ();
		butler.NewGame ();
	}

	IButler GetButler (GameType gameType)
	{
		switch (gameType) {
		case GameType.ONE_PLAYER_DEMO:
			return gameObject.GetComponent<ButlerDemo> ();
		case GameType.TWO_PLAYER_PLAY_GAMES:
			return gameObject.GetComponent<ButlerPlayGames> ();
		case GameType.NONE_SELECTED:
		default:
			throw new NotImplementedException ();
		}
		return butler;
	}

	void HandleGameStateChanged (GameState state)
	{
		switch (state) {
		case GameState.SELECTING_GAME_TYPE:
			SceneMaster.instance.LoadAsync (SceneMaster.SCENE_MAIN_MENU);
			break;
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
			break;
		case GameState.PLAYING:
			SceneMaster.instance.LoadAsync (SceneMaster.SCENE_GAME);
			break;
		default:
			throw new NotImplementedException ();
		}
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


	public void NewGame ()
	{
		butler.NewGame ();
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
