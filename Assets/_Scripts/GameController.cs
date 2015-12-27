using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour
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
	MyRealTimeMultiplayerListener multiplayerListener;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (this);
			return;
		}
		instance = this;
		layerTileTheirs = new LayerInfo ("Tile Theirs");
		layerBoatTheirs = new LayerInfo ("Boat Theirs");
		source = GetComponent<AudioSource> ();
		multiplayerListener = new MyRealTimeMultiplayerListener ();

//		InitNearby ();
	}

	void InitNearby ()
	{
		Debug.logger.Log ("Initializing nearby connections …");
		PlayGamesPlatform.InitializeNearby ((client) => {
			Debug.logger.Log ("Nearby connections initialized: client=" + client);
		});
	}

	void Start ()
	{
		InitPlayGamesPlatform ();
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


		Debug.logger.Log ("Activating PlayGamesPlatform …");
		PlayGamesPlatform.Activate ();


		// authenticate user:
		Social.localUser.Authenticate ((bool success) => {
			// handle success or failure
			Debug.logger.Log ("Authenticate --> " + (success ? "SUCCESS" : "FAILURE"));
			if (success) {
				CreateMultiplayerRoom ();
			}
		});
	}

	void CreateMultiplayerRoom ()
	{
		Debug.logger.Log ("Creating multiplayer room …");
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: multiplayerListener);
	}

	public void OnRoomConnected (bool success)
	{
		if (success) {
			StartNewGame ();
		}
	}

	public void StartNewGame ()
	{
		boatPlacementController.RecreateBoats ();
	}

	public void EndGame ()
	{
		boatPlacementController.DestroyBoats ();
		CreateMultiplayerRoom ();
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

}
