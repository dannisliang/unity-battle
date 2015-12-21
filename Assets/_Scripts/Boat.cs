using UnityEngine;

public class Boat
{
	public bool horizontal;
	public Position[] positions;

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

	public float GetX ()
	{
		return positions [0].x;
	}

	public float GetZ ()
	{
		return positions [0].z;
	}

	public override string ToString ()
	{
		return (horizontal ? "Horizontal" : "Vertical") + "Boat(" + positions [0] + "," + positions [1] + ")";
	}

}
