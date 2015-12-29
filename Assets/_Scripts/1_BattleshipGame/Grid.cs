using UnityEngine;
using System.Collections;

[System.Serializable]
public class Grid
{

	static int[] boatSizes = { 5, 4, 3, 2, 2, 1 };

	public Boat[] boats;

	public void RandomizeBoats ()
	{
		boats = new Boat[boatSizes.Length];
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
		}
	}

	public bool IsHit (Position position)
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
