using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoatPlacementController : MonoBehaviour
{
	static int[] boatSizes = { 5, 4, 3, 2, 2, 1 };
	public class Boat
	{
		public bool horizontal;
		public Coord[] coords;

		public Boat (int size)
		{
			horizontal = Random.value > .5f;
			int u = Random.Range (0, Utils.GRID_SIZE - size + 1);
			int v = Random.Range (0, Utils.GRID_SIZE);

			//Debug.Log ("u=" + u + " v=" + v);
			coords = MakeCoordinates (u, v, size, horizontal);
		}

		public float GetX ()
		{
			return (coords [0].x + coords [coords.Length - 1].x) / 2f;
		}

		public float GetZ ()
		{
			return (coords [0].z + coords [coords.Length - 1].z) / 2f;
		}

		public override string ToString ()
		{
			return (horizontal ? "Horizontal" : "Vertical") + "Boat(" + coords [0] + "," + coords [1] + ")";
		}

	}

	public class Coord
	{
		public int x;
		public int z;

		public Coord (int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public override bool Equals (System.Object obj)
		{
			if (obj == null) {
				return false;
			}
			Coord other = (Coord)obj;
			return other.x == x && other.z == z;
		}

		public override int GetHashCode ()
		{
			return x * 31 + z;
		}

		public override string ToString ()
		{
			return "" + ((char)(x + 65)) + (z + 1);
		}
	}

	static int[] boatSizes = { 5, 4, 3, 3, 2 };
>>>>>>> 28ace24... generated ships don't collide

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
				for (int j = 0; j < boat.coords.Length && !conflict; j++) {
					Coord c = boat.coords [j];
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

	bool IsHit (Coord coord)
	{
		for (int i = 0; i < boats.Length; i++) {
			if (boats [i] == null) {
				continue;
			}
			for (int j = 0; j < boats [i].coords.Length; j++) {
				if (coord.Equals (boats [i].coords [j])) {
					return true;
				}
			}
		}
		return false;
	}

	static Coord[] MakeCoordinates (int u, int v, int size, bool horizontal)
	{
		Coord[] locations = new Coord[size];
		for (int i = 0; i < size; i++) {
			locations [i] = new Coord (horizontal ? u + i : v, horizontal ? v : u + i);
		}
		return locations;
	}

}
