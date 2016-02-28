using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(CardboardAudioSource))]
public class BattleController : MonoBehaviour
{
	public static BattleController instance { get; private set; }

	public static LayerInfo layerGridTheirs;

	public Animator whoseTurnAnimator;
	public GameObject rocketOursPrefab;
	public GameObject rocketTheirsPrefab;
	public GameObject markerTargetReticleOursAtTheirsPrefab;
	public GameObject markerHitPrefab;
	public GameObject markerMissPrefab;
	public GameObject gridOurs;
	public GameObject gridTheirs;
	public GameObject rocketOriginTheirs;
	public BoatPlacementController boatsOursPlacementController;
	public BoatPlacementController boatsTheirsPlacementController;
	public AudioClip noFireClip;
	public CenterPanelController centerPanelController;

	bool playing;
	int _firing;
	Whose? loser;
	Whose whoseTurn = Whose.Ours;

	int firing {
		get {
			return _firing;
		}
		set {
			_firing = value;
			if (firing > 0) {
				aimReticleAtTheirs.SetActive (false);
			}
			AnnounceGameState ();
		}
	}

	object BattleStateLock = new System.Object ();

	public delegate void BattleState (bool playing, bool firing, Whose? loser);

	private event BattleState _OnBattleState;

	public event BattleState OnBattleState {
		add {
			lock (BattleStateLock) {
				_OnBattleState += value;
				value (playing, firing > 0, loser);
			}
		}
		remove {
			lock (BattleStateLock) {
				_OnBattleState -= value;
			}
		}
	}

	public delegate void ReticleIdentify (Boat boat);

	public event ReticleIdentify OnReticleIdentify;

	public delegate void ReticleAim (Whose whose, Position position);

	public event ReticleAim OnReticleAim;

	public delegate void StrikeOccurred (Whose whose, Boat boat, Position position, StrikeResult result);

	public event StrikeOccurred OnStrikeOccurred;

	public delegate void WhoseTurn (Whose whoseTurn);

	public event WhoseTurn OnWhoseTurn;

	CardboardAudioSource source;
	GameObject aimReticleAtOurs;
	GameObject aimReticleAtTheirs;
	GameObject targetReticleOurs;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		layerGridTheirs = new LayerInfo ("Grid Theirs");
		source = GetComponent<CardboardAudioSource> ();

		targetReticleOurs = Instantiate (markerTargetReticleOursAtTheirsPrefab);
		targetReticleOurs.name += " targetReticleOurs";

		aimReticleAtOurs = Instantiate (markerTargetReticleOursAtTheirsPrefab);
		aimReticleAtOurs.name += " aimReticleAtOurs";

		aimReticleAtTheirs = Instantiate (markerTargetReticleOursAtTheirsPrefab);
		aimReticleAtTheirs.name += " aimReticleAtTheirs";
	}

	void OnEnable ()
	{
		Debug.Log ("***" + typeof(BattleController) + ".OnEnable()");
		playing = false;
		loser = null;
		_firing = 0;

		AimAt (Whose.Theirs, null); // reticle starts disabled
		AimAt (Whose.Ours, null); // reticle starts disabled
		TargetAt (null); // reticle starts disabled
		boatsOursPlacementController.RecreateBoats (SystemInfo.deviceUniqueIdentifier);
		SendOurBoatPositions ();
	}

	void SendOurBoatPositions ()
	{
		RealtimeBattle.EncodeAndSendGrid (boatsOursPlacementController.grid);
	}

	public void SetBoatsTheirs (string playerUniqueId, Boat[] boats)
	{
		playing = true;
		Whose whoseStarts = playerUniqueId.CompareTo (SystemInfo.deviceUniqueIdentifier) > 0 ? Whose.Ours : Whose.Theirs;
		Debug.Log ("***FIRST TURN: " + whoseStarts);
		StartCoroutine (SetTurn (whoseStarts));
		boatsTheirsPlacementController.SetBoats (playerUniqueId, boats);
		AnnounceGameState ();
	}

	void AnnounceGameState ()
	{
		if (_OnBattleState != null) {
			_OnBattleState (playing, firing > 0, loser);
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
		if (loser != null) {
			return;
		}
		if (firing > 0) {
			return;
		}
		if (whoseTurn != Whose.Ours) {
			return;
		}
		PlaceMarker (whose, position, Marker.Aim);
		if (OnReticleAim != null) {
			OnReticleAim (whose, position);
		}
	}

	public bool FireAt (Position targetPosition, bool tileHasBeenFiredUpon)
	{
		if (!IsGridReady ()) {
			return false;
		}
		if (tileHasBeenFiredUpon) {
			source.PlayOneShot (noFireClip);
			return false;
		}
		firing++;
		TargetAt (targetPosition);
		RealtimeBattle.EncodeAndSendLaunch (targetPosition);
		LaunchRocket (Whose.Theirs, targetPosition, delegate {
			firing--;
			TargetAt (null);
		});
		return true;
	}

	public bool IsGridReady ()
	{
		if (!playing) {
			return false;
		}
		if (loser != null) {
			return false;
		}
		if (firing > 0) { // && !Application.isEditor) {
			return false;
		}
		if (whoseTurn != Whose.Ours) {
			return false;
		}
		return true;
	}

	public void LaunchRocket (Whose atWhose, Position targetPosition, Action callback)
	{
		GameObject rocket = Game.instance.InstantiateTemp (atWhose == Whose.Theirs ? rocketOursPrefab : rocketTheirsPrefab);
		Vector3 localPos = targetPosition.AsGridLocalPosition (Marker.Aim);
		
		Transform originTransform = (atWhose == Whose.Theirs ? Camera.main.transform : rocketOriginTheirs.transform);
		PosRot start = new PosRot (FirePos (originTransform), originTransform.rotation);

		Transform targetGridTransform = (atWhose == Whose.Theirs ? gridTheirs : gridOurs).transform.parent;
		Vector3 pos = targetGridTransform.position + (targetGridTransform.rotation * localPos);
		pos += targetGridTransform.right * .5f + targetGridTransform.up * .5f;
		PosRot end = new PosRot (pos, targetGridTransform.rotation);

		rocket.GetComponent<RocketController> ().Launch (atWhose, targetPosition, start, end, callback);
		if (atWhose == Whose.Ours) {
			centerPanelController.IssueWarning (.5f, 2f);
		}
	}

	Vector3 FirePos (Transform originTransform)
	{
		return originTransform.position + Utils.RandomSign () * originTransform.right;
	}

	public void AimAt (Position position)
	{
		if (whoseTurn == Whose.Ours && firing == 0) {
			PlaceMarker (Whose.Ours, position, Marker.Aim);
		}
	}

	public void TargetAt (Position position)
	{
		PlaceMarker (Whose.Theirs, position, Marker.Target);
	}

	IEnumerator SetTurn (Whose whose)
	{
		whoseTurn = whose;
		yield return new WaitForSeconds (1.5f);
		whoseTurnAnimator.SetInteger ("WhoseTurnInt", (int)whose);
		if (OnWhoseTurn != null) {
			OnWhoseTurn (whose);
		}
	}

	public StrikeResult Strike (Whose whose, Position position)
	{
		StrikeResult result = _Strike (whose, position);
		StartCoroutine (SetTurn (whose));
		CheckAllBoatsSunk (whose);
		return result;
	}

	public StrikeResult _Strike (Whose whose, Position position)
	{
		BoatPlacementController boatPlacementController = whose == Whose.Theirs ? boatsTheirsPlacementController : boatsOursPlacementController;
		Boat boat;
		StrikeResult result = boatPlacementController.grid.FireAt (position, out boat);
		if (!Application.isEditor) {
			Debug.Log ("***Strike(" + position + ") -> " + result);
		}
		switch (result) {
		case StrikeResult.IGNORED_ALREADY_MISSED:
		case StrikeResult.IGNORED_ALREADY_HIT:
			break;
		case StrikeResult.MISS:
			PlaceMarker (whose, position, Marker.Miss);
			break;
		case StrikeResult.HIT_NOT_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			AnnounceStrike (whose, boat, position, result);
			break;
		case StrikeResult.HIT_AND_SUNK:
			PlaceMarker (whose, position, Marker.Hit);
			PlaceSunkBoat (whose, boat);
			AnnounceStrike (whose, boat, position, result);
			break;
		default:
			throw new System.NotImplementedException ();
		}
		return result;
	}

	void AnnounceStrike (Whose whose, Boat boat, Position position, StrikeResult result)
	{
		if (OnStrikeOccurred != null) {
			OnStrikeOccurred (whose, boat, position, result);
		}
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
				Game.instance.QuitGame ();
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
		case Marker.Target:
			go = targetReticleOurs;
			break;
		case Marker.Aim:
			go = whose == Whose.Theirs ? aimReticleAtTheirs : aimReticleAtOurs;
			break;
		case Marker.Hit:
			go = Game.instance.InstantiateTemp (markerHitPrefab);
			break;
		case Marker.Miss:
			go = Game.instance.InstantiateTemp (markerMissPrefab);
			break;
		}
		go.transform.SetParent (whose == Whose.Theirs ? gridTheirs.transform : gridOurs.transform, false);

		if (marker == Marker.Aim) {
			go.GetComponent<TargetReticleController> ().SetTargetPosition (position, false);
		} else if (marker == Marker.Target) {
			go.GetComponent<TargetReticleController> ().SetTargetPosition (position, true);
		} else {
			go.transform.localPosition = position.AsGridLocalPosition (marker);
		}
	}

}
