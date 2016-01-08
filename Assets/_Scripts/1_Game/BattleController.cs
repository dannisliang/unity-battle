using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BattleController : MonoBehaviour
{
	public static BattleController instance { get; private set; }

	public static LayerInfo layerTileTheirs;

	public GameObject rocketPrefab;
	public GameObject markerAimReticlePrefab;
	public GameObject markerHitPrefab;
	public GameObject markerMissPrefab;
	public GameObject reticle;
	public GameObject gridOurs;
	public GameObject gridTheirs;
	public BoatPlacementController boatsOursPlacementController;
	public BoatPlacementController boatsTheirsPlacementController;
	public AudioClip waterPlopClip;
	public AudioClip shipExplosionClip;
	public AudioClip noFireClip;

	public delegate void GameState (bool playing, bool firing);

	public event GameState OnGameState;

	public delegate void ReticleIdentify (Boat boat);

	public delegate void ReticleAim (Whose whose, Position position);

	public event ReticleIdentify OnReticleIdentify;
	public event ReticleAim OnReticleAim;

	bool playing;
	bool firing;
	AudioSource source;
	GameObject aimReticle;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		layerTileTheirs = new LayerInfo ("Tile Theirs");
		source = GetComponent<AudioSource> ();
		aimReticle = Instantiate (markerAimReticlePrefab);
		AimAt (Whose.Theirs, null); // reticle starts disabled
	}

	void Start ()
	{
		Assert.IsTrue (GameController.butler.IsSignedIn () && GameController.butler.IsGameConnected ());
		if (GameController.butler.IsSignedIn () && GameController.butler.IsGameConnected ()) {
			boatsOursPlacementController.RecreateBoats ();
			SendOurBoatPositions ();
		}
	}

	void SendOurBoatPositions ()
	{
		RealtimeBattle.EncodeAndSend (boatsOursPlacementController.grid);
	}

	public void SetBoatsTheirs (Boat[] boats)
	{
		playing = true;
		boatsTheirsPlacementController.SetBoats (boats);
		AnnounceGameState ();
	}

	void AnnounceGameState ()
	{
		if (OnGameState != null) {
			OnGameState (playing, firing);
		}
	}

	public void Identify (Boat boat, Position position)
	{
		if (OnReticleIdentify != null) {
			OnReticleIdentify.Invoke (boat);
		}

	}

	public void AimAt (Whose whose, Position position)
	{
		if (OnReticleAim != null) {
			OnReticleAim (whose, position);
		}
		aimReticle.SetActive (position != null);
		if (position != null) {
			PlaceMarker (whose, position, Marker.Aim);
		}
	}

	public void FalseFire ()
	{
		if (!playing || firing) {
			return;
		}
		source.PlayOneShot (noFireClip);
	}

	public bool FireAt (Transform targetTransform)
	{
		if (!playing || firing) {
			return false;
		}
		GameObject rocket = Instantiate (rocketPrefab);
		BattleController.instance.SetIsFiring (true);
		rocket.GetComponent<RocketController> ().Launch (Camera.main.transform, targetTransform, delegate {
			SetIsFiring (false);
		});
		return true;
	}

	public StrikeResult Strike (Whose whose, Position position)
	{
		BoatPlacementController boatPlacementController = whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController;
		Boat boat;
		StrikeResult result = boatPlacementController.grid.FireAt (position, out boat);
		Debug.Log ("***Strike(" + position + ") -> " + result);
		switch (result) {
		case StrikeResult.IGNORED_ALREADY_MISSED:
			source.PlayOneShot (waterPlopClip);
			break;
		case StrikeResult.IGNORED_ALREADY_HIT:
			SceneMaster.instance.Async (delegate {
				source.PlayOneShot (shipExplosionClip);
			}, 1f);
			break;
		case StrikeResult.MISS:
			PlaceMarker (whose, position, Marker.Miss);
			source.PlayOneShot (waterPlopClip);
			break;
		case StrikeResult.HIT_NOT_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			SceneMaster.instance.Async (delegate {
				source.PlayOneShot (shipExplosionClip);
			}, 1f);
			break;
		case StrikeResult.HIT_AND_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			PlaceSunkBoat (whose, boat);
			SceneMaster.instance.Async (delegate {
				source.PlayOneShot (shipExplosionClip);
			}, 1f);
			break;
		default:
			throw new System.NotImplementedException ();
		}
		return result;
	}

	void PlaceSunkBoat (Whose whose, Boat boat)
	{
		(whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController).PlaceBoat (boat, true);
	}

	void PlaceMarker (Whose whose, Position position, Marker marker)
	{
		GameObject go = null;
		float zPos = 0f;
		switch (marker) {
		case Marker.Aim:
			go = aimReticle;
			zPos = -Utils.BOAT_HEIGHT - 2f * Utils.CLEARANCE_HEIGHT;
			break;
		case Marker.Hit:
			go = Instantiate (markerHitPrefab);
			zPos = -.5f * Utils.BOAT_HEIGHT - Utils.CLEARANCE_HEIGHT;
			break;
		case Marker.Miss:
			go = Instantiate (markerMissPrefab);
			zPos = -.5f * Utils.BOAT_HEIGHT - Utils.CLEARANCE_HEIGHT;
			break;
		}
		go.transform.SetParent (whose == Whose.Theirs ? gridTheirs.transform : gridOurs.transform, false);
		go.transform.localPosition = new Vector3 (position.x, Utils.GRID_SIZE - 1f - position.y, zPos);
	}

	void SetIsFiring (bool firing)
	{
		this.firing = firing;
		reticle.SetActive (!firing);
		AnnounceGameState ();
	}

}
