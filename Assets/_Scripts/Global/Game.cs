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

public class Game : MonoBehaviour//,IDiscoveryListener,IMessageListener
{

	public static Game instance;

	public GameObject mainMenuGameObject;
	public GameObject viewModeGameObject;
	public GameObject viewModePlayingGameObject;
	public GameObject playingGameObject;
	public ButlerAi butlerAi;
	public ButlerPlayGames butlerPlayGames;
	public GameStateTextController gameStateTextController;
	public CardboardAssistantController cardboardAssistantController;

	BaseButler butler;

	GameState masterGameState = GameState.AUTHENTICATING;

	public Dictionary<GameState, List<GameObject>> activationDict;

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

	List<GameObject> tempObjects;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
		tempObjects = new List<GameObject> ();

		MakeGameObjectsActive ();

//		InitNearby ();

		OnGameStateChange += HandleGameStateChanged;
	}

	public GameObject InstantiateTemp (UnityEngine.Object original)
	{
		GameObject go = Instantiate (original) as GameObject;
		tempObjects.Add (go);
		return go;
	}

	void MakeGameObjectsActive ()
	{
		Debug.Log ("***MakeGameObjectsActive() masterGameState=" + masterGameState);
		switch (masterGameState) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.GAME_WAS_TORN_DOWN:
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
			viewModeGameObject.SetActive (false);
			viewModePlayingGameObject.SetActive (false);
			playingGameObject.SetActive (false);
			mainMenuGameObject.SetActive (true);
			break;
		case GameState.SELECTING_VIEW_MODE:
			mainMenuGameObject.SetActive (false);
			playingGameObject.SetActive (false);
			viewModeGameObject.SetActive (true);
			viewModePlayingGameObject.SetActive (true);
			cardboardAssistantController.Recenter ();
			break;
		case GameState.PLAYING:
			mainMenuGameObject.SetActive (false);
			viewModeGameObject.SetActive (false);
			viewModePlayingGameObject.SetActive (true);
			playingGameObject.SetActive (true);
			cardboardAssistantController.Recenter ();
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	public void QuitGame ()
	{
		Debug.Log ("*** ** ** ** ** ** ** Quitting game NOW ** ** ** ** ** **");
		butler.QuitGame ();
	}

	void HandleButlerGameStateChange (GameState state)
	{
		if (_OnGameStateChange != null) {
			_OnGameStateChange (state);
		}
	}

	void SetActiveButler (GameType gameType)
	{
		if (butler != null) {
			butler.OnGameStateChange -= HandleButlerGameStateChange;
		}
		switch (gameType) {
		case GameType.ONE_PLAYER_DEMO:
			butler = butlerAi;
			break;
		case GameType.TWO_PLAYER_PLAY_GAMES:
//			butler = butlerFirebase;
			butler = butlerPlayGames;
			break;
		default:
			throw new NotImplementedException ();
		}
		butler.OnGameStateChange += HandleButlerGameStateChange;
	}

	void HandleGameStateChanged (GameState state)
	{
		if (masterGameState == state) {
			Debug.Log ("*** ignoring GameState switch as we're already in " + masterGameState);
			return;
		}
		Debug.Log ("***===> GameState=" + state + " (was " + masterGameState + ")");
		masterGameState = state;
		switch (state) {
		case GameState.GAME_WAS_TORN_DOWN:
			if (butler != null) {
				Assert.IsTrue (butler.enabled);
				butler.enabled = false;
				butler = null;
			}
			foreach (GameObject go in tempObjects) {
				Destroy (go);
			}
			break;
		case GameState.SELECTING_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.PLAYING:
		case GameState.SELECTING_VIEW_MODE:
			MakeGameObjectsActive ();
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	public void SelectViewMode (bool? vrMode)
	{
		if (vrMode == null) {
			Assert.AreEqual (GameState.PLAYING, butler.GetGameState ());
			butler.PauseGamePlay ();
		} else {
			Assert.AreEqual (GameState.SELECTING_VIEW_MODE, butler.GetGameState ());
			cardboardAssistantController.VrModeChanged ((bool)vrMode);
			butler.StartGamePlay ();
		}
	}

	//	void HardRestart ()
	//	{
	//		Debug.Log ("***Destroying " + gameObject + " …");
	//		Destroy (gameObject);
	//		SceneMaster.HardRestart ();
	//	}

	#if UNITY_EDITOR
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Alpha0)) {
			Time.timeScale = Time.timeScale > 5f ? 1f : 10f;
			Debug.Log ("Time.timeScale -> " + Time.timeScale);
		}
		if (Input.GetKeyDown (KeyCode.S) && Input.GetKey (KeyCode.RightShift)) {
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


	public void NewGame (GameType gameType)
	{
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, masterGameState);
		SetActiveButler (gameType);
		Assert.IsFalse (butler.enabled);
		butler.enabled = true;
		butler.NewGame ();
	}

	public void SetErrorFailureReasonText (string failureReasonText)
	{
		gameStateTextController.SetFailureReasonText (failureReasonText);
	}


	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		if (masterGameState != GameState.SELECTING_VIEW_MODE && masterGameState != GameState.PLAYING) {
			Debug.Log ("***Ingoring '" + Convert.ToChar (data [0]) + "' real time message received due to game state " + masterGameState);
			return;
		}
		RealtimeBattle.DecodeAndExecute (data, isReliable);
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
