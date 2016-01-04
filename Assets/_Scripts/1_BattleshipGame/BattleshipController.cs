using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BattleshipController : MonoBehaviour
{
	public static BattleshipController instance { get; private set; }

	public static LayerInfo layerTileTheirs;
	public static LayerInfo layerBoatTheirs;

	public GameObject rocketPrefab;
	public GameObject markerAimReticlePrefab;
	public GameObject markerHitPrefab;
	public GameObject markerMissPrefab;
	public GameObject reticle;
	public GameObject gridOurs;
	public GameObject gridTheirs;
	public FireAtWillController fireAtWillController;
	public BoatPlacementController boatsOursPlacementController;
	public BoatPlacementController boatsTheirsPlacementController;
	public AudioClip waterPlopClip;
	public AudioClip shipExplosionClip;

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
		layerBoatTheirs = new LayerInfo ("Boat Theirs");
		source = GetComponent<AudioSource> ();
		aimReticle = Instantiate (markerAimReticlePrefab);
		AimAt (Whose.Theirs, null); // reticle starts disabled
	}

	void Start ()
	{
		if (GameController.gamesPlatform.IsAuthenticated () && GameController.gamesPlatform.RealTime.IsRoomConnected ()) {
			boatsOursPlacementController.RecreateBoats ();
			GameController.instance.SendOurBoatPositions ();
		}
	}

	public void AimAt (Whose whose, Position position)
	{
		aimReticle.SetActive (position != null);
		fireAtWillController.SetVisible (position != null);
		if (position != null) {
			PlaceMarker (whose, position, Marker.Aim);
		}
	}

	public void Strike (Whose whose, Position position)
	{
		BoatPlacementController boatPlacementController = whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController;
		Boat boat;
		StrikeResult result = boatPlacementController.grid.FireAt (position, out boat);
		Debug.Log ("***Strike(" + position + ") -> " + result);
		switch (result) {
		case StrikeResult.IGNORED_ALREADY_MISSED:
			BattleshipController.instance.PlayWaterPlop ();
			break;
		case StrikeResult.IGNORED_ALREADY_HIT:
			GameController.instance.ExecuteDelayed (delegate {
				PlayShipExplosion ();
			}, 1f);
			break;
		case StrikeResult.MISS:
			PlaceMarker (whose, position, Marker.Miss);
			BattleshipController.instance.PlayWaterPlop ();
			break;
		case StrikeResult.HIT_NOT_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			GameController.instance.ExecuteDelayed (delegate {
				PlayShipExplosion ();
			}, 1f);
			break;
		case StrikeResult.HIT_AND_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			GameController.instance.ExecuteDelayed (delegate {
				PlayShipExplosion ();
				PlaceSunkBoat (whose, boat);
			}, 1f);
			break;
		default:
			throw new System.NotImplementedException ();
		}
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

	public void PlayWaterPlop ()
	{
		source.PlayOneShot (waterPlopClip);
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
		if (firing) {
			fireAtWillController.enabled = false;
		}
	}

}
