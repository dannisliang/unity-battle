using UnityEngine;
using System.Collections;

[System.Serializable]
public class Grid
{
	public struct BoatConfiguration
	{
		public int size;
		public string designation;

		public BoatConfiguration (int size, string designation)
		{
			this.size = size;
			this.designation = designation;
		}
	}

	// http://www.navy.mil/navydata/our_ships.asp
	public static BoatConfiguration[] fleet = {
		new BoatConfiguration (5, "Aircraft Carrier"),
		new BoatConfiguration (4, "Amphibious Assault"),
		new BoatConfiguration (3, "Cruiser"),
		new BoatConfiguration (2, "Littoral Combat"),
		new BoatConfiguration (2, "Destroyer"),
		new BoatConfiguration (1, "Submarine"),
	};

	public Boat[] boats;

	public void RandomizeBoats ()
	{
		boats = new Boat[fleet.Length];
		for (int i = 0; i < fleet.Length; i++) {
			int size = fleet [i].size;

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

	public override string ToString ()
	{
		string[] arr = new string[boats.Length];
		for (int i = 0; i < boats.Length; i++) {
			arr [i] = boats [i].ToString ();
		}
		return "Grid(" + string.Join (",", arr) + ")";
	}
}
