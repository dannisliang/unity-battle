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
		CreateGameObjects (grid);
	}

	public void CreateGameObjects (Grid grid)
	{
		this.grid = grid;
		DestroyBoats ();
		CreateBoats ();
	}

	public void DestroyBoats ()
	{
		Utils.DestroyChildren (transform);
	}

	void CreateBoats ()
	{
		for (int i = 0; i < grid.boats.Length; i++) {
			GameObject clone = Instantiate (boatPrefab) as GameObject;
			clone.transform.SetParent (transform, false);

			BoatController boatController = clone.GetComponent<BoatController> ();
			boatController.Configure (grid.boats [i]);

			Utils.SetNoSaveNoEditHideFlags (clone.transform);
#if UNITY_EDITOR
			Undo.RegisterCreatedObjectUndo (clone, "Create " + grid.boats [i]);
#endif
		}
	}

}
