using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoatPlacementController : MonoBehaviour
{
	public Whose whose;
	public GameObject boatNormalPrefab;
	public GameObject boatSunkPrefab;

	public Grid grid { get; private set; }

	void Awake ()
	{
		grid = new Grid ();
	}

	public void RecreateBoats ()
	{
		SetBoats (null);
		CreateBoats ();
	}

	public void SetBoats (Boat[] boats)
	{
		this.grid.SetBoats (whose, boats);
		DestroyBoats ();
	}

	public void DestroyBoats ()
	{
		Utils.DestroyChildren (transform);
	}

	void CreateBoats ()
	{
		for (int i = 0; i < grid.boats.Length; i++) {
			PlaceBoat (grid.boats [i], false);
		}
	}

	public void PlaceBoat (Boat boat, bool sunk)
	{
		GameObject clone = Instantiate (sunk ? boatSunkPrefab : boatNormalPrefab) as GameObject;
		clone.transform.SetParent (transform, false);

		BoatController boatController = clone.GetComponent<BoatController> ();
		boatController.Configure (boat, sunk);

		//Utils.SetNoSaveNoEditHideFlags (clone.transform);
#if UNITY_EDITOR
		Undo.RegisterCreatedObjectUndo (clone, "Create " + boat);
#endif
	}
		
}
