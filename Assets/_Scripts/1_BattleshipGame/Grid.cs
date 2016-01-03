using UnityEngine;
using UnityEngine.Assertions;
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

	int[,] misses;

	public void RandomizeBoats ()
	{
		misses = new int[Utils.GRID_SIZE, Utils.GRID_SIZE];
		boats = new Boat[fleet.Length];
		for (int i = 0; i < fleet.Length; i++) {
			int size = fleet [i].size;

			bool conflict = true;
			while (conflict) {
				Boat boat = new Boat (size);

				conflict = false;
				for (int j = 0; j < boat.positions.Length && !conflict; j++) {
					Position pos = boat.positions [j];
					Boat otherBoat;
					StrikeResult result = FireAt (pos, out otherBoat, testOnly: true);
					switch (result) {
					case StrikeResult.MISS:
					case StrikeResult.IGNORED_ALREADY_MISSED:
						continue;
					case StrikeResult.IGNORED_ALREADY_HIT:
					case StrikeResult.HIT_NOT_SUNK:
					case StrikeResult.HIT_AND_SUNK:
						conflict = true;
						break;
					default:
						throw new System.NotImplementedException ();
					}
				}
				if (!conflict) {
					boats [i] = boat;
				}
			}
		}
	}

	public StrikeResult FireAt (Position position, out Boat boat, bool testOnly = false)
	{
		for (int i = 0; i < boats.Length; i++) {
			if (boats [i] == null) {
				continue;
			}
			StrikeResult result = boats [i].FireAt (position, testOnly);
			if (result == StrikeResult.IGNORED_ALREADY_HIT) {
				boat = boats [i];
				return result;
			} else if (result == StrikeResult.MISS) {
				continue;
			} else {
				Assert.IsTrue (result == StrikeResult.HIT_AND_SUNK || result == StrikeResult.HIT_NOT_SUNK);
				boat = boats [i];
				return result;
			}
		}
		if (misses [position.x, position.y] > 0) {
			boat = null;
			return StrikeResult.IGNORED_ALREADY_MISSED;
		}
		misses [position.x, position.y]++;
		boat = null;
		return StrikeResult.MISS;
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
