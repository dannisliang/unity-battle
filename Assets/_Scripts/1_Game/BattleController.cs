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
	public GameObject rocketOriginTheirs;
	public GridController gridOursController;
	public GridController gridTheirsController;
	public AudioClip noFireClip;
	public CenterPanelController centerPanelController;

	int _firing;
	Whose loser;
	Whose whoseTurn = Whose.Nobody;

	int firing {
		get {
			return _firing;
		}
		set {
			_firing = value;
			if (firing > 0) {
				gridOursController.HideAimReticle ();
				gridTheirsController.HideAimReticle ();
			}
			AnnounceBattleState ();
		}
	}

	object BattleStateLock = new System.Object ();

	public delegate void BattleState (Whose whoseTurn, bool firing, Whose loser);

	private event BattleState _OnBattleState;

	public event BattleState OnBattleState {
		add {
			lock (BattleStateLock) {
				_OnBattleState += value;
				value (whoseTurn, firing > 0, loser);
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

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		layerGridTheirs = new LayerInfo ("Grid Theirs");
		source = GetComponent<CardboardAudioSource> ();
	}

	void OnEnable ()
	{
		Game.instance.OnGameStateChange += GameStateChanged;
	}

	void OnDiable ()
	{
		Game.instance.OnGameStateChange -= GameStateChanged;
	}

	void GameStateChanged (GameState state)
	{
		switch (state) {
		case GameState.SETTING_UP_GAME:
			Init ();
			break;
		case GameState.PLAYING:
			SendOurBoatPositions ();
			break;
		case GameState.SELECTING_GAME_TYPE:
		case GameState.TEARING_DOWN_GAME:
		case GameState.GAME_WAS_TORN_DOWN:
			StopAllCoroutines ();
			break;
		case GameState.AUTHENTICATING:
		case GameState.SELECTING_VIEW_MODE:
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	public void Init ()
	{
		Debug.Log ("***" + typeof(BattleController) + ".Init()");
		whoseTurn = Whose.Nobody;
		loser = Whose.Nobody;
		_firing = 0;

		// reset grids
		gridOursController.Init (SystemInfo.deviceUniqueIdentifier);
		gridTheirsController.Init (null);
		// tell everyone to reset state
		AnnounceBattleState ();
	}

	void SendOurBoatPositions ()
	{
		RealtimeBattle.EncodeAndSendGrid (gridOursController.grid);
	}

	public void SetBoatsTheirs (string playerUniqueId, Boat[] boats)
	{
		Whose whoseStarts = playerUniqueId.Equals (Utils.AI_PLAYER_ID) || playerUniqueId.CompareTo (SystemInfo.deviceUniqueIdentifier) > 0 ? Whose.Ours : Whose.Theirs;
//		Debug.Log ("***FIRST TURN: " + whoseStarts + " playerUniqueId=" + playerUniqueId + " deviceUniqueIdentifier=" + SystemInfo.deviceUniqueIdentifier);
		StartCoroutine (SetTurn (whoseStarts));
		gridTheirsController.SetBoats (playerUniqueId, boats);
		AnnounceBattleState ();
	}

	void AnnounceBattleState ()
	{
		if (_OnBattleState != null) {
			_OnBattleState (whoseTurn, firing > 0, loser);
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
		if (loser != Whose.Nobody) {
			return;
		}
		if (firing > 0) {
			return;
		}
		if (whoseTurn != Whose.Ours) {
			return;
		}
		GetGrid (whose).SetAimPosition (position);
		if (OnReticleAim != null) {
			OnReticleAim (whose, position);
		}
	}

	public void TargetAt (Whose whose, Position position)
	{
		GetGrid (whose).SetTargetPosition (position);
	}

	GridController GetGrid (Whose whose)
	{
		return whose == Whose.Theirs ? gridTheirsController : gridOursController;
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
		TargetAt (Whose.Theirs, targetPosition);
		RealtimeBattle.EncodeAndSendLaunch (targetPosition);
		LaunchRocket (Whose.Theirs, targetPosition);
		return true;
	}

	public bool IsGridReady ()
	{
		if (whoseTurn == Whose.Nobody) {
			return false;
		}
		if (loser != Whose.Nobody) {
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

	public void LaunchRocket (Whose atWhose, Position targetPosition)
	{
		GameObject rocket = Game.InstantiateTemp (atWhose == Whose.Theirs ? rocketOursPrefab : rocketTheirsPrefab);
		Vector3 localPos = targetPosition.AsGridLocalPosition (Marker.Aim);
		
		Transform originTransform = (atWhose == Whose.Theirs ? Camera.main.transform : rocketOriginTheirs.transform);
		PosRot start = new PosRot (FirePos (originTransform), originTransform.rotation);

		Transform targetGridTransform = (atWhose == Whose.Theirs ? gridTheirsController : gridOursController).transform.parent;
		Vector3 pos = targetGridTransform.position + (targetGridTransform.rotation * localPos);
		pos += targetGridTransform.right * .5f + targetGridTransform.up * .5f;
		PosRot end = new PosRot (pos, targetGridTransform.rotation);

		firing++;
		rocket.GetComponent<RocketController> ().Launch (atWhose, targetPosition, start, end, delegate {
			firing--;
		});
		if (atWhose == Whose.Ours) {
			centerPanelController.IssueWarning (.5f, 2f);
		}
	}

	Vector3 FirePos (Transform originTransform)
	{
		return originTransform.position + Utils.RandomSign () * originTransform.right;
	}

	IEnumerator SetTurn (Whose whose)
	{
		whoseTurn = whose;
		if (whose != Whose.Nobody) {
			yield return new WaitForSeconds (1.5f);
		}
		whoseTurnAnimator.SetInteger ("WhoseTurnInt", (int)whose);
		if (OnWhoseTurn != null) {
			OnWhoseTurn (whose);
		}
		AnnounceBattleState ();
	}

	public StrikeResult Strike (Whose whose, Position position)
	{
		StrikeResult result = _Strike (whose, position);
		Whose nextTurn = CheckAllBoatsSunk (whose);
		StartCoroutine (SetTurn (nextTurn));
		return result;
	}

	public StrikeResult _Strike (Whose whose, Position position)
	{
		Boat boat;
		StrikeResult result = GetGrid (whose).grid.FireAt (position, out boat);
		if (!Application.isEditor) {
			Debug.Log ("***Strike(" + position + ") -> " + result);
		}
		switch (result) {
		case StrikeResult.IGNORED_ALREADY_MISSED:
		case StrikeResult.IGNORED_ALREADY_HIT:
			break;
		case StrikeResult.MISS:
			GetGrid (whose).SetMarker (position, Marker.Miss);
			break;
		case StrikeResult.HIT_NOT_SUNK:
			GetGrid (whose).SetMarker (position, Marker.Hit);
			AnnounceStrike (whose, boat, position, result);
			break;
		case StrikeResult.HIT_AND_SUNK:
			GetGrid (whose).SetMarker (position, Marker.Hit);
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

	Whose CheckAllBoatsSunk (Whose whose)
	{
		if (loser != Whose.Nobody) {
			return Whose.Nobody;
		}
		if (GetGrid (whose).grid.AllBoatsSunk ()) {
			loser = whose;
			AnnounceBattleState ();
			SceneMaster.instance.Async (delegate {
				Game.instance.QuitGame ();
			}, Utils.RESTART_DELAY);
			return Whose.Nobody;
		}
		return whose;
	}

	void PlaceSunkBoat (Whose whose, Boat boat)
	{
		(whose == Whose.Theirs ? gridTheirsController : gridOursController).PlaceBoat (boat, true);
	}

}
