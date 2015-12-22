using UnityEngine;

[System.Serializable]
public class Boat
{
	public bool horizontal{ get; private set; }

	public Position[] positions{ get; private set; }

	static Position[] MakeBoatPositions (int u, int v, int size, bool horizontal)
	{
		Position[] locations = new Position[size];
		for (int i = 0; i < size; i++) {
			locations [i] = new Position (horizontal ? u + i : v, horizontal ? v : u + i);
		}
		return locations;
	}

	public Boat (int size)
	{
		horizontal = Random.value > .5f;
		int u = Random.Range (0, Utils.GRID_SIZE - size + 1);
		int v = Random.Range (0, Utils.GRID_SIZE);

		//Debug.Log ("u=" + u + " v=" + v);
		positions = MakeBoatPositions (u, v, size, horizontal);
	}

	public int Size ()
	{
		return positions.Length;
	}

	public Position GetPosition (int index)
	{
		return positions [index];
	}

	public override string ToString ()
	{
		return (horizontal ? "Horizontal" : "Vertical") + "Boat(" + positions [0] + "," + positions [1] + ")";
	}

}
