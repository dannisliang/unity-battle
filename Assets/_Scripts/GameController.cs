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
	public Text statusLogText;

	bool firing;
	AudioSource source;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (this);
			return;
		}
		instance = this;
		InitLogger ();
		layerTileTheirs = new LayerInfo ("Tile Theirs");
		layerBoatTheirs = new LayerInfo ("Boat Theirs");
		source = GetComponent<AudioSource> ();

		InitNearby ();
	}

	void InitLogger ()
	{
		Utils.logger = new Logger (new UiTextLogHandler (statusLogText));
	}

	void InitNearby ()
	{
		Utils.logger.Log ("Initializing nearby connections …");
		PlayGamesPlatform.InitializeNearby ((client) => {
			Utils.logger.Log ("Nearby connections initialized: client=" + client);
		});
	}

	void Start ()
	{
		InitPlayGamesPlatform ();
	}

	void InitPlayGamesPlatform ()
	{
		Utils.logger.Log ("Initializing PlayGamesPlatform …");

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


		Utils.logger.Log ("Activating PlayGamesPlatform …");
		PlayGamesPlatform.Activate ();


		// authenticate user:
		Social.localUser.Authenticate ((bool success) => {
			// handle success or failure
			Utils.logger.Log ("Authenticate --> " + (success ? "SUCCESS" : "FAILURE"));
			if (success) {
				CreateGame ();
			}
		});
	}

	void CreateGame ()
	{
		Utils.logger.Log ("Creating game …");
		MyRealTimeMultiplayerListener listener = new MyRealTimeMultiplayerListener ();
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: listener);
	}

	public void StartNewGame ()
	{
		boatPlacementController.RecreateBoats ();
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
