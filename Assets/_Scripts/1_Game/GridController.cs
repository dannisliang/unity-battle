using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridController : MonoBehaviour
{
	public Whose whose;
	public GameObject boatNormalPrefab;
	public GameObject boatSunkPrefab;
	public GameObject reticleGazePrefab;
	public GameObject reticleTargetingPrefab;
	public GameObject reticleTargetLockedPrefab;
	public GameObject markerHitPrefab;
	public GameObject markerMissPrefab;
	public GameObject rocketPrefab;
	public GameObject boatHolder;
	public GameObject rocketOrigin;

	public Grid grid { get; private set; }

	BoatController[] boatControllers;
	GameObject reticleGaze;
	GameObject reticleTargeting;
	GameObject reticleTargetLocked;
	Whose whoseTurn;
	bool firing;
	Whose loser;
	Position aimPosition;

	void OnEnable ()
	{
		BattleController.instance.OnBattleState += BattleStateUpdated;
	}

	void OnDisable ()
	{
		BattleController.instance.OnBattleState -= BattleStateUpdated;
	}

	void BattleStateUpdated (Whose whoseTurn, bool firing, Whose loser)
	{
		this.whoseTurn = whoseTurn;
		this.firing = firing;
		this.loser = loser;
		UpdateAimGazeReticles ();
	}

	void UpdateAimGazeReticles ()
	{
		reticleGaze.SetActive (aimPosition != null && loser == Whose.Nobody && (whoseTurn != Whose.Ours || firing));
		reticleTargeting.SetActive (aimPosition != null && loser == Whose.Nobody && whoseTurn == Whose.Ours && !firing);
	}

	public void Init (string playerUniqueId)
	{
		boatControllers = new BoatController[Grid.fleet.Length];
		CreateReticles ();
		SetBoats (playerUniqueId, null);
		if (whose == Whose.Ours) {
			CreateBoats ();
		}
	}

	void CreateReticles ()
	{
		reticleGaze = Game.InstantiateTemp (reticleGazePrefab);
		reticleGaze.transform.SetParent (transform, false);
		reticleGaze.name += " reticle gaze " + whose;

		reticleTargeting = Game.InstantiateTemp (reticleTargetingPrefab);
		reticleTargeting.transform.SetParent (transform, false);
		reticleTargeting.name += " reticle targeting " + whose;

		reticleTargetLocked = Game.InstantiateTemp (reticleTargetLockedPrefab);
		reticleTargetLocked.transform.SetParent (transform, false);
		reticleTargetLocked.name += " reticle target locked " + whose;
	}

	public void SetBoats (string playerUniqueId, Boat[] boats)
	{
		grid = new Grid (playerUniqueId);
		grid.SetBoats (whose, boats);
		DestroyBoats ();
	}

	void DestroyBoats ()
	{
		Utils.DestroyChildren (boatHolder.transform);
	}

	void CreateBoats ()
	{
		for (int i = 0; i < grid.boats.Length; i++) {
			boatControllers [i] = PlaceBoat (grid.boats [i], false);
		}
	}

	public RocketController MakeRocket ()
	{
		GameObject rocket = Game.InstantiateTemp (rocketPrefab, rocketOrigin.transform);
		rocket.transform.position = MakeFirePos (rocketOrigin.transform);
		return rocket.GetComponent<RocketController> ();
	}

	Vector3 MakeFirePos (Transform originTransform)
	{
		return originTransform.position + Utils.RandomSign () * originTransform.right;
	}

	public void HideAimGazeReticles ()
	{
		reticleGaze.SetActive (false);
		reticleTargeting.SetActive (false);
	}

	public void ShowAimGazeReticles ()
	{
		reticleGaze.SetActive (true);
		reticleTargeting.SetActive (true);
	}

	public void HideTargetReticle ()
	{
		reticleTargetLocked.SetActive (false);
	}

	public void SetTargetPosition (Position position)
	{
		reticleTargetLocked.SetActive (true);
		reticleTargetLocked.GetComponent<ReticleTargetLockedController> ().SetPosition (position);
	}

	public void SetAimPosition (Position position)
	{
		aimPosition = position;
		reticleGaze.GetComponent<ReticleGazeController> ().SetTargetPosition (position);
		reticleTargeting.GetComponent<ReticleTargetingController> ().SetTargetPosition (position);
		UpdateAimGazeReticles ();
	}

	public StrikeResult Strike (Position position, out Boat boat)
	{
		StrikeResult result = grid.FireAt (position, out boat);
		switch (result) {
		case StrikeResult.IGNORED_ALREADY_MISSED:
		case StrikeResult.IGNORED_ALREADY_HIT:
			break;
		case StrikeResult.MISS:
			SetMarker (position, Marker.Miss);
			break;
		case StrikeResult.HIT_NOT_SUNK:
			SetMarker (position, Marker.Hit);
			break;
		case StrikeResult.HIT_AND_SUNK:
			SetMarker (position, Marker.Hit);
			PlaceBoat (boat, true);
			break;
		default:
			throw new System.NotImplementedException ();
		}
		if (boatControllers [0] != null) {
			for (int i = 0; i < grid.boats.Length; i++) {
				boatControllers [i].UpdateDamage ();
			}
		}
		return result;
	}

	void SetMarker (Position position, Marker marker)
	{
		GameObject go = Game.InstantiateTemp (marker == Marker.Hit ? markerHitPrefab : markerMissPrefab, transform);
		go.transform.localPosition = position.AsGridLocalPosition (marker);
	}

	BoatController PlaceBoat (Boat boat, bool sunk)
	{
		GameObject prefab = sunk ? boatSunkPrefab : boatNormalPrefab;
		GameObject clone = Game.InstantiateTemp (prefab);
		clone.transform.SetParent (boatHolder.transform, false);

		BoatController boatController = clone.GetComponent<BoatController> ();
		boatController.Configure (boat, sunk);

		//Utils.SetNoSaveNoEditHideFlags (clone.transform);
		#if UNITY_EDITOR
		Undo.RegisterCreatedObjectUndo (clone, "Create " + boat);
		#endif
		return boatController;
	}

}
