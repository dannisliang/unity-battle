using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent (typeof(AudioSource))]
public class BattleController : MonoBehaviour
{
	public static BattleController instance { get; private set; }

	public static LayerInfo layerTileTheirs;

	public GameObject rocketOursPrefab;
	public GameObject rocketTheirsPrefab;
	public GameObject markerAimReticleTheirsAtOursPrefab;
	public GameObject markerAimReticleOursAtTheirsPrefab;
	public GameObject markerHitPrefab;
	public GameObject markerMissPrefab;
	public GameObject reticle;
	public GameObject gridOurs;
	public GameObject gridTheirs;
	public GameObject rocketOriginTheirs;
	public BoatPlacementController boatsOursPlacementController;
	public BoatPlacementController boatsTheirsPlacementController;
	public AudioClip waterPlopClip;
	public AudioClip shipExplosionClip;
	public AudioClip noFireClip;

	public delegate void GameState (bool playing, bool firing, Whose? loser);

	public event GameState OnGameState;

	public delegate void ReticleIdentify (Boat boat);

	public delegate void ReticleAim (Whose whose, Position position);

	public event ReticleIdentify OnReticleIdentify;
	public event ReticleAim OnReticleAim;

	bool playing;
	bool firing;
	Whose? loser;
	AudioSource source;
	GameObject aimReticleOurs;
	GameObject aimReticleTheirs;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		layerTileTheirs = new LayerInfo ("Tile Theirs");
		source = GetComponent<AudioSource> ();
		aimReticleOurs = Instantiate (markerAimReticleTheirsAtOursPrefab);
		aimReticleTheirs = Instantiate (markerAimReticleOursAtTheirsPrefab);
		AimAt (Whose.Theirs, null); // reticle starts disabled
		AimAt (Whose.Ours, null); // reticle starts disabled
	}

	void Start ()
	{
		boatsOursPlacementController.RecreateBoats ();
		SendOurBoatPositions ();
	}

	void SendOurBoatPositions ()
	{
		RealtimeBattle.EncodeAndSendGrid (boatsOursPlacementController.grid);
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
			OnGameState (playing, firing, loser);
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
		PlaceMarker (whose, position, Marker.Aim);
	}

	public void FalseFire ()
	{
		if (!playing || firing) {
			return;
		}
		source.PlayOneShot (noFireClip);
	}

	public bool FireAt (Position targetPosition)
	{
		if (!playing || firing) {
			return false;
		}
		BattleController.instance.SetIsFiring (true);
		RealtimeBattle.EncodeAndSendLaunch (targetPosition);
		LaunchRocket (Whose.Theirs, targetPosition, delegate {
			SetIsFiring (false);
		});
		return true;
	}

	public void LaunchRocket (Whose atWhose, Position targetPosition, Action callback)
	{
		GameObject rocket = Instantiate (atWhose == Whose.Theirs ? rocketOursPrefab : rocketTheirsPrefab);
		Vector3 localPos = targetPosition.AsGridLocalPosition (Marker.Aim);
		
		Transform originTransform = (atWhose == Whose.Theirs ? Camera.main.transform : rocketOriginTheirs.transform);
		PosRot start = new PosRot (FirePos (originTransform), originTransform.rotation);

		Transform targetGridTransform = (atWhose == Whose.Theirs ? gridTheirs : gridOurs).transform;
		Vector3 pos = targetGridTransform.position + (targetGridTransform.rotation * localPos);
		pos += targetGridTransform.right * .5f + targetGridTransform.up * .5f;
		PosRot end = new PosRot (pos, targetGridTransform.rotation);

		rocket.GetComponent<RocketController> ().Launch (atWhose, targetPosition, start, end, callback);
	}

	Vector3 FirePos (Transform originTransform)
	{
		return originTransform.position + Utils.RandomSign () * originTransform.right;
	}

	public void AimAt (Position position)
	{
		PlaceMarker (Whose.Ours, position, Marker.Aim);
	}

	public StrikeResult Strike (Whose whose, Position position)
	{
		StrikeResult result = _Strike (whose, position);
		CheckAllBoatsSunk (whose);
		return result;
	}

	public StrikeResult _Strike (Whose whose, Position position)
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
			source.PlayOneShot (shipExplosionClip);
			break;
		case StrikeResult.MISS:
			PlaceMarker (whose, position, Marker.Miss);
			source.PlayOneShot (waterPlopClip);
			break;
		case StrikeResult.HIT_NOT_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			source.PlayOneShot (shipExplosionClip);
			break;
		case StrikeResult.HIT_AND_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			PlaceSunkBoat (whose, boat);
			source.PlayOneShot (shipExplosionClip);
			break;
		default:
			throw new System.NotImplementedException ();
		}
		return result;
	}

	void CheckAllBoatsSunk (Whose whose)
	{
		if (loser != null) {
			return;
		}
		BoatPlacementController boatPlacementController = whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController;
		if (boatPlacementController.grid.AllBoatsSunk ()) {
			loser = whose;
			AnnounceGameState ();
			SceneMaster.instance.Async (delegate {
				Game.instance.Restart ();
			}, Utils.RESTART_DELAY);
		}
	}

	void PlaceSunkBoat (Whose whose, Boat boat)
	{
		(whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController).PlaceBoat (boat, true);
	}

	void PlaceMarker (Whose whose, Position position, Marker marker)
	{
		GameObject go = null;
		switch (marker) {
		case Marker.Aim:
			go = whose == Whose.Theirs ? aimReticleTheirs : aimReticleOurs;
			break;
		case Marker.Hit:
			go = Instantiate (markerHitPrefab);
			break;
		case Marker.Miss:
			go = Instantiate (markerMissPrefab);
			break;
		}
		go.transform.SetParent (whose == Whose.Theirs ? gridTheirs.transform : gridOurs.transform, false);
		if (marker == Marker.Aim) {
			go.GetComponent<AimReticleController> ().SetTargetPosition (position);
		} else {
			go.transform.localPosition = position.AsGridLocalPosition (marker);
		}
	}

	void SetIsFiring (bool firing)
	{
		this.firing = firing;
		reticle.SetActive (!firing);
		if (firing) {
			aimReticleTheirs.SetActive (false);
		}
		AnnounceGameState ();
	}

}
