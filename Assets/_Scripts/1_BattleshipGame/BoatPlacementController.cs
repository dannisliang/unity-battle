using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoatPlacementController : MonoBehaviour
{

	public GameObject boatPrefab;

	public Grid grid { get; private set; }

	public void RecreateBoats ()
	{
		Grid grid = new Grid ();
		grid.RandomizeBoats ();
		SetGrid (grid);
		CreateBoats ();
	}

	public void SetGrid (Grid grid)
	{
		this.grid = grid;
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

	public void PlaceBoat (Boat boat, bool aboveMarkers)
	{
		GameObject clone = Instantiate (boatPrefab) as GameObject;
		clone.transform.SetParent (transform, false);

		BoatController boatController = clone.GetComponent<BoatController> ();
		boatController.Configure (boat, aboveMarkers);

			Utils.SetNoSaveNoEditHideFlags (clone.transform);
#if UNITY_EDITOR
		Undo.RegisterCreatedObjectUndo (clone, "Create " + boat);
#endif
	}
}
