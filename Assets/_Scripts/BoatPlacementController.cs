using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoatPlacementController : MonoBehaviour
{

	static int[] boatSizes = { 5, 4, 3, 2, 2, 1 };

	public GameObject boatPrefab;

	Boat[] boats;

	public void RecreateBoats ()
	{
		boats = new Boat[boatSizes.Length];
		Utils.DestroyChildren (transform);
		CreateBoats ();
	}

	void CreateBoats ()
	{
		Vector3 pos = Vector3.zero;
		Vector3 scale = Vector3.one;
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

			pos.x = boats [i].GetX () - .5f * Utils.GRID_SIZE + .5f;
			pos.z = Utils.GRID_SIZE - .5f - boats [i].GetZ ();

			scale.x = boats [i].horizontal ? size : 1f;
			scale.z = boats [i].horizontal ? 1f : size;

			GameObject clone = Instantiate (boatPrefab) as GameObject;
			clone.transform.SetParent (transform, false);
			clone.transform.localPosition = pos;
			clone.transform.localScale = scale;
			clone.name += " (" + size + " units)";
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
