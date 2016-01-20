using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

[System.Serializable]
public class Grid
{
	[System.NonSerialized] public Whose whose;

	public delegate void StrikeOccurred (Whose whose, Boat boat, Position position, StrikeResult result);

	[field:System.NonSerialized] public event StrikeOccurred OnStrikeOccurred;

	public delegate void GridSetup ();

	[field:System.NonSerialized] public event GridSetup OnGridSetup;

	// http://www.navy.mil/navydata/our_ships.asp
	public static BoatConfiguration[] fleet = {
//		new BoatConfiguration (5, "Aircraft Carrier"),
//		new BoatConfiguration (4, "Amphibious Assault"),
		new BoatConfiguration (3, "Cruiser"),
//		new BoatConfiguration (2, "Littoral Combat"),
//		new BoatConfiguration (2, "Destroyer"),
//		new BoatConfiguration (1, "Submarine"),
	};

	public Boat[] boats;

	[System.NonSerialized] int[,] misses;

	public int getMisses ()
	{
		int m = 0;
		for (int i = 0; i < Utils.GRID_SIZE; i++) {
			for (int j = 0; j < Utils.GRID_SIZE; j++) {
				if (misses [i, j] > 0) {
					m++;
				}
			}
		}
		return m;
	}

	public void SetBoats (Whose whose, Boat[] boats)
	{
		this.whose = whose;
//		Debug.Log ("Grid(" + whose + ") misses = new int[" + Utils.GRID_SIZE + "," + Utils.GRID_SIZE + "]");
		misses = new int[Utils.GRID_SIZE, Utils.GRID_SIZE];
		if (boats == null) {
			MakeRandomizedBoats (whose);
		} else {
			for (int i = 0; i < fleet.Length; i++) {
				boats [i].whose = whose;
			}
			this.boats = boats;
		}
		if (OnGridSetup != null) {
			OnGridSetup ();
		}
	}

	Boat[] MakeRandomizedBoats (Whose whose)
	{
		boats = new Boat[fleet.Length];
		for (int i = 0; i < fleet.Length; i++) {
			bool conflict = true;
			while (conflict) {
				Boat boat = new Boat (whose, fleet [i]);

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
		return boats;
	}

	public bool AllBoatsSunk ()
	{
		for (int i = 0; i < boats.Length; i++) {
			if (!boats [i].IsSunk ()) {
				return false;
			}
		}
		return true;
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
				if (!testOnly && OnStrikeOccurred != null) {
					OnStrikeOccurred (whose, boats [i], position, result);
				}
				return result;
			}
		}
		if (misses [position.x, position.y] > 0) {
			boat = null;
			return StrikeResult.IGNORED_ALREADY_MISSED;
		}
		if (!testOnly) {
			misses [position.x, position.y]++;
		}
		boat = null;
		if (!testOnly && OnStrikeOccurred != null) {
			OnStrikeOccurred (whose, boat, position, StrikeResult.MISS);
		}
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
