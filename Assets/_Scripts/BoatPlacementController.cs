using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoatPlacementController : MonoBehaviour
{

	static int[] boatSizes = { 5, 4, 3, 2, 2, 1 };

	public GameObject boatPrefab;

	Boat[] boats;

	void Awake ()
	{
		RecreateBoats ();
	}

	public void RecreateBoats ()
	{
		boats = new Boat[boatSizes.Length];
		Utils.DestroyChildren (transform);
		CreateBoats ();
	}

	void CreateBoats ()
	{
		for (int i = 0; i < boatSizes.Length; i++) {
			int size = boatSizes [i];

			bool conflict = true;
			while (conflict) {
				Boat boat = new Boat (size);

				conflict = false;
				for (int j = 0; j < boat.positions.Length && !conflict; j++) {
					Position c = boat.positions [j];
					if (IsHit (c)) {
						conflict = true;
					}
				}
				if (!conflict) {
					boats [i] = boat;
				}
			}
				
			GameObject clone = Instantiate (boatPrefab) as GameObject;
			clone.transform.SetParent (transform, false);

			BoatController boatController = clone.GetComponent<BoatController> ();
			boatController.Configure (boats [i]);

			Utils.SetNoSaveNoEditHideFlags (clone.transform);
#if UNITY_EDITOR
			Undo.RegisterCreatedObjectUndo (clone, "Create " + boats [i]);
#endif
		}
	}

	bool IsHit (Position position)
	{
		for (int i = 0; i < boats.Length; i++) {
			if (boats [i] == null) {
				continue;
			}
			for (int j = 0; j < boats [i].positions.Length; j++) {
				if (position.Equals (boats [i].positions [j])) {
					return true;
				}
			}
		}
		return false;
	}

}
