using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour,RealTimeMultiplayerListener
{
	public static GameController instance;
	public static LayerInfo layerTileTheirs;
	public static LayerInfo layerBoatTheirs;

	public GameObject boatHitPrefab;
	public GameObject boatMissPrefab;
	public GameObject gridTheirs;
	public GameObject reticle;
	public BoatPlacementController boatPlacementController;
	public AudioClip waterPlopClip;
	public AudioClip shipExplosionClip;

	bool firing;
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
		layerTileTheirs = new LayerInfo ("Tile Theirs");
		layerBoatTheirs = new LayerInfo ("Boat Theirs");
		source = GetComponent<AudioSource> ();

//		InitNearby ();
	}

	void OnApplicationPause (bool pause)
	{
		Debug.Log ("***OnApplicationPause(" + pause + ") --> " + (pause ? "PAUSED" : "RESUMING"));
		Debug.Log ("***IsRoomConnected==" + PlayGamesPlatform.Instance.RealTime.IsRoomConnected () + "]");
		if (!PlayGamesPlatform.Instance.RealTime.IsRoomConnected ()) {
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

	public void PlayWaterPlop ()
	{
		source.PlayOneShot (waterPlopClip);
	}

	public void PlayShipExplosionAfter (float delay)
	{
		Invoke ("PlayShipExplosion", delay);
	}

	public void PlayShipExplosion ()
	{
		source.PlayOneShot (shipExplosionClip);
	}

	public bool IsFiring ()
	{
		return firing;
	}

	public void SetIsFiring (bool firing)
	{
		this.firing = firing;
		reticle.SetActive (!firing);
	}

	public void PlaceMarker (Position position, bool hit)
	{
		GameObject marker = Instantiate (hit ? boatHitPrefab : boatMissPrefab);
		marker.transform.SetParent (gridTheirs.transform, false);
		marker.transform.localPosition = new Vector3 (position.x, Utils.GRID_SIZE - 1f - position.y, 0f);
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
			SceneManager.LoadScene (1);
//			boatPlacementController.RecreateBoats ();
			PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, new byte[] { 1, 2, 3 });
		}
	}

	// RealTimeMultiplayerListener
	public void OnLeftRoom ()
	{
		Debug.Log ("***OnLeftRoom()");
		SceneManager.LoadScene (0);
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
