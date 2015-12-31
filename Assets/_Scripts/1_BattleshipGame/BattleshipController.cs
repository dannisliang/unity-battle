using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BattleshipController : MonoBehaviour
{
	public static BattleshipController instance { get; private set; };
	public static LayerInfo layerTileTheirs;
	public static LayerInfo layerBoatTheirs;

	public GameObject reticle;
	public GameObject rocketPrefab;
	public GameObject boatHitPrefab;
	public GameObject boatMissPrefab;
	public GameObject gridOurs;
	public GameObject gridTheirs;
	public BoatPlacementController boatsOursPlacementController;
	public BoatPlacementController boatsTheirsPlacementController;
	public AudioClip waterPlopClip;
	public AudioClip shipExplosionClip;

	bool firing;
	AudioSource source;

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
	}

	void Start ()
	{
		if (GameController.gamesPlatform.IsAuthenticated () && GameController.gamesPlatform.RealTime.IsRoomConnected ()) {
			boatsOursPlacementController.RecreateBoats ();
			GameController.instance.SendOurBoatPositions ();
		}
	}

	public void Strike (Whose whose, Position position)
	{
		BoatPlacementController boatPlacementController = whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController;
		bool hit = boatPlacementController.grid.IsHit (position);
		PlaceMarker (whose, position, hit);
		if (hit) {
			BattleshipController.instance.PlayShipExplosionAfter (1f);
		} else {
			BattleshipController.instance.PlayWaterPlop ();
		}
	}

	void PlaceMarker (Whose whose, Position position, bool hit)
	{
		GameObject marker = Instantiate (hit ? boatHitPrefab : boatMissPrefab);
		marker.transform.SetParent (whose == Whose.Theirs ? gridTheirs.transform : gridOurs.transform, false);
		marker.transform.localPosition = new Vector3 (position.x, Utils.GRID_SIZE - 1f - position.y, -Utils.BOAT_HEIGHT);
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

}
