using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BattleshipController : MonoBehaviour
{
	public static BattleshipController instance { get; private set; }

	public static LayerInfo layerTileTheirs;
	public static LayerInfo layerBoatTheirs;

	public GameObject rocketPrefab;
	public GameObject AimReticlePrefab;
	public GameObject boatHitPrefab;
	public GameObject boatMissPrefab;
	public GameObject reticle;
	public GameObject gridOurs;
	public GameObject gridTheirs;
	public Text fireAtWillText;
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
		aimReticle = Instantiate (AimReticlePrefab);
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
		fireAtWillText.enabled = position != null;
		if (position != null) {
			PlaceMarker (whose, position, Marker.Aim);
		}
	}

	public void Strike (Whose whose, Position position)
	{
		BoatPlacementController boatPlacementController = whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController;
		bool hit = boatPlacementController.grid.IsHit (position);
		PlaceMarker (whose, position, hit ? Marker.Hit : Marker.Miss);
		if (hit) {
			BattleshipController.instance.PlayShipExplosionAfter (1f);
		} else {
			BattleshipController.instance.PlayWaterPlop ();
		}
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
			go = Instantiate (boatHitPrefab);
			zPos = -.5f * Utils.BOAT_HEIGHT - Utils.CLEARANCE_HEIGHT;
			break;
		case Marker.Miss:
			go = Instantiate (boatMissPrefab);
			zPos = -.5f * Utils.BOAT_HEIGHT - Utils.CLEARANCE_HEIGHT;
			break;
		}
		go.transform.SetParent (whose == Whose.Theirs ? gridTheirs.transform : gridOurs.transform, false);
		go.transform.localScale = new Vector3 (1f, 1f, Utils.BOAT_HEIGHT);
		go.transform.localPosition = new Vector3 (position.x, Utils.GRID_SIZE - 1f - position.y, zPos);
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
		if (firing) {
			fireAtWillText.enabled = false;
		}
	}

}
