using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(CardboardAudioSource))]
public class BattleController : MonoBehaviour
{
	static string CATEGORY = typeof(BattleController).Name;

	public static BattleController instance { get; private set; }

	public static LayerInfo layerGridTheirs;

	public Animator whoseTurnAnimator;
	public GridController gridOursController;
	public GridController gridTheirsController;
	public AudioClip noFireClip;
	public CenterPanelController centerPanelController;

	GoogleAnalyticsV4 gav4;
	int _firing;
	Whose loser;
	Whose whoseTurn = Whose.Nobody;
	bool boatPositionsSent;

	int firing {
		get {
			return _firing;
		}
		set {
			_firing = value;
			if (firing == 0) {
				gridOursController.HideTargetReticle ();
				gridTheirsController.HideTargetReticle ();
			}
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

	public delegate void ReticleAim (Whose whose, Position position);

	public event ReticleAim OnReticleAim;

	public delegate void StrikeOccurred (Whose whose, Boat boat, Position position, StrikeResult result);

	public event StrikeOccurred OnStrikeOccurred;

	public delegate void WhoseTurn (Whose whoseTurn);

	public event WhoseTurn OnWhoseTurn;

	CardboardAudioSource source;

	void Awake ()
	{
		gav4 = AnalyticsAssistant.gav4;
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
//		gav4.LogEvent (CATEGORY, "OnEnable", null, 0);
		Game.instance.OnGameStateChange += GameStateChanged;
	}

	void OnDiable ()
	{
//		gav4.LogEvent (CATEGORY, "OnDisable", null, 0);
		Game.instance.OnGameStateChange -= GameStateChanged;
	}

	void GameStateChanged (GameState state)
	{
		switch (state) {
		case GameState.SETTING_UP_GAME:
			Init ();
			break;
		case GameState.PLAYING:
			if (!boatPositionsSent) {
				SendOurBoatPositions ();
				boatPositionsSent = true;
			}
			break;
		case GameState.TEARING_DOWN_GAME:
			LogStats ();
			break;
		case GameState.GAME_WAS_TORN_DOWN:
			StopAllCoroutines ();
			break;
		case GameState.SELECTING_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.SELECTING_VIEW_MODE:
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	public void Init ()
	{
//		gav4.LogEvent (CATEGORY, "Init", null, 0);
		Debug.Log ("***" + typeof(BattleController) + ".Init()");
		whoseTurn = Whose.Nobody;
		loser = Whose.Nobody;
		_firing = 0;

		// reset grids
		boatPositionsSent = false;
		gridOursController.Init (SystemInfo.deviceUniqueIdentifier);
		gridTheirsController.Init (null);
		// tell everyone to reset state
		AnnounceBattleState ();
	}

	void LogStats ()
	{
		LogStats (gridTheirsController.grid);
		LogStats (gridOursController.grid);
	}

	void LogStats (Grid grid)
	{
		Boat[] boats = grid.boats;
		int units = 0;
		int sunk = 0;
		int hits = 0;
		int misses = grid.getMisses ();
		for (int i = 0; i < boats.Length; i++) {
			BoatConfiguration config = boats [i].config;
			units += config.size;
			hits += boats [i].HitCount ();
			if (boats [i].IsSunk ()) {
				sunk++;
			}
		}
		gav4.LogEvent (CATEGORY, grid.whose.ToString () + "-HitsOutOf" + units, null, hits);
		gav4.LogEvent (CATEGORY, grid.whose.ToString () + "-SunkOutOf" + boats.Length, null, sunk);
		gav4.LogEvent (CATEGORY, grid.whose.ToString () + "-AccuracyPercent", null, 100 * hits / (hits + misses));
	}

	void SendOurBoatPositions ()
	{
		gav4.LogEvent (CATEGORY, "BoatPositions-Ours", null, 0);
		RealtimeBattle.EncodeAndSendGrid (gridOursController.grid);
	}

	public void SetBoatsTheirs (string playerUniqueId, Boat[] boats)
	{
		gav4.LogEvent (CATEGORY, "BoatPositions-Theirs", null, 0);
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
		GetGridController (whose).SetAimPosition (position);
		if (OnReticleAim != null) {
			OnReticleAim (whose, position);
		}
	}

	public bool FireAt (Position targetPosition, bool tileHasBeenFiredUpon)
	{
//		gav4.LogEvent (CATEGORY, "FireAt", tileHasBeenFiredUpon.ToString (), 0);
		if (!IsGridReady ()) {
			return false;
		}
		if (tileHasBeenFiredUpon) {
			source.PlayOneShot (noFireClip);
			return false;
		}
		GetGridController (Whose.Theirs).SetTargetPosition (targetPosition);
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
//		gav4.LogEvent (CATEGORY, "LaunchRocket", atWhose.ToString (), 0);
		RocketController rocketController = GetGridController (atWhose).MakeRocket ();
		Vector3 localTargetPos = targetPosition.AsGridLocalPosition (Marker.Aim);

		PosRot start = new PosRot (rocketController.transform);

		Transform targetGridTransform = GetGridController (atWhose).transform;
		Vector3 pos = targetGridTransform.position + (targetGridTransform.rotation * localTargetPos);
		pos += targetGridTransform.right * .5f + targetGridTransform.up * .5f;
		PosRot end = new PosRot (pos, targetGridTransform.rotation);

		firing++;
		rocketController.Launch (atWhose, targetPosition, start, end, delegate {
			firing--;
		});
		if (atWhose == Whose.Ours) {
			centerPanelController.IssueWarning (.5f, 2f);
		}
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
//		gav4.LogEvent (CATEGORY, "Strike", null, 0);
//		gav4.LogEvent (CATEGORY, "Strike-" + whose.ToString () + "-" + result.ToString (), null, 1);
		Whose nextTurn = CheckAllBoatsSunk (whose);
		StartCoroutine (SetTurn (nextTurn));
		return result;
	}

	public StrikeResult _Strike (Whose whose, Position position)
	{
		Boat boat;
		StrikeResult result = GetGridController (whose).Strike (position, out boat);
		if (!Application.isEditor) {
			Debug.Log ("***Strike(" + position + ") -> " + result);
		}
		AnnounceStrike (whose, boat, position, result);
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
		if (GetGridController (whose).grid.AllBoatsSunk ()) {
			loser = whose;
			gav4.LogEvent (CATEGORY, "AnnounceLoser", "Loser-" + loser.ToString (), 0);
			AnnounceBattleState ();
			SceneMaster.instance.Async (delegate {
				Game.instance.QuitGame ("AnnounceLoser");
			}, Utils.RESTART_DELAY);
			return Whose.Nobody;
		}
		return whose;
	}

	GridController GetGridController (Whose whose)
	{
		return whose == Whose.Theirs ? gridTheirsController : gridOursController;
	}

}
