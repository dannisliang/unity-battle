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
	public GameObject reticlePrefab;
	public GameObject markerHitPrefab;
	public GameObject markerMissPrefab;
	public GameObject boatHolder;

	public Grid grid { get; private set; }

	BoatController boatController;
	GameObject aimReticle;
	GameObject targetReticle;

	public void Init (string playerUniqueId)
	{
		CreateReticles ();
		SetBoats (playerUniqueId, null);
		if (whose == Whose.Ours) {
			CreateBoats ();
		}
	}

	void CreateReticles ()
	{
		aimReticle = Game.InstantiateTemp (reticlePrefab);
		aimReticle.transform.SetParent (transform, false);
		aimReticle.name += " aim reticle " + whose;

		targetReticle = Game.InstantiateTemp (reticlePrefab);
		targetReticle.transform.SetParent (transform, false);
		targetReticle.name += " target reticle " + whose;
	}

	public void SetBoats (string playerUniqueId, Boat[] boats)
	{
		grid = new Grid (playerUniqueId);
		grid.SetBoats (whose, boats);
		DestroyBoats ();
	}

	public void DestroyBoats ()
	{
		Utils.DestroyChildren (boatHolder.transform);
	}

	void CreateBoats ()
	{
		for (int i = 0; i < grid.boats.Length; i++) {
			PlaceBoat (grid.boats [i], false);
		}
	}

	public void PlaceBoat (Boat boat, bool sunk)
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
	}

	public void HideAimReticle ()
	{
		aimReticle.SetActive (false);
	}

	public void SetTargetPosition (Position position)
	{
		targetReticle.SetActive (true);
		targetReticle.GetComponent<TargetReticleController> ().SetTargetPosition (position, true);
	}

	public void SetAimPosition (Position position)
	{
		aimReticle.SetActive (true);
		aimReticle.GetComponent<TargetReticleController> ().SetTargetPosition (position, false);
	}

	public void SetMarker (Position position, Marker marker)
	{
		GameObject go = Game.InstantiateTemp (marker == Marker.Hit ? markerHitPrefab : markerMissPrefab, transform);
		go.transform.localPosition = position.AsGridLocalPosition (marker);
	}

}
