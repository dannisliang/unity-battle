﻿using UnityEngine;

[System.Serializable]
public class Boat
{
	public bool horizontal{ get; private set; }

	public Position[] positions{ get; private set; }

	int size;
	int[] hits;

	public Boat (int size)
	{
		this.size = size;
		horizontal = Random.value > .5f;
		int u = Random.Range (0, Utils.GRID_SIZE - size + 1);
		int v = Random.Range (0, Utils.GRID_SIZE);

		positions = MakeBoatPositions (u, v, size, horizontal);
		hits = new int[size];
	}

	public StrikeResult FireAt (Position position, bool testOnly = false)
	{
		for (int i = 0; i < size; i++) {
			if (positions [i].Equals (position)) {
				if (hits [i] > 0) {
					return StrikeResult.IGNORED_ALREADY_HIT;
				}
				if (!testOnly) {
					hits [i]++;
				}
				return IsSunk () ? StrikeResult.HIT_AND_SUNK : StrikeResult.HIT_NOT_SUNK;
			}
		}
		return StrikeResult.MISS;
	}

	public bool IsSunk ()
	{
		for (int i = 0; i < size; i++) {
			if (hits [i] == 0) {
				return false;
			}
		}
		return true;
	}

	public int Size ()
	{
		return positions.Length;
	}

	public Position GetPosition (int index)
	{
		return positions [index];
	}

	static Position[] MakeBoatPositions (int u, int v, int size, bool horizontal)
	{
		Position[] locations = new Position[size];
		for (int i = 0; i < size; i++) {
			locations [i] = new Position (horizontal ? u + i : v, horizontal ? v : u + i);
		}
		return locations;
	}

	public override string ToString ()
	{
		return (horizontal ? "Horizontal" : "Vertical") + "Boat(" + positions [0] + ")";
	}

}
