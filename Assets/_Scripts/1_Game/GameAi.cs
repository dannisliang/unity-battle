using UnityEngine;
using System.Collections.Generic;

public class GameAi
{
	List<Position> emptyPositions;

	public GameAi ()
	{
		emptyPositions = new List<Position> ();
		for (int i = 0; i < Utils.GRID_SIZE; i++) {
			for (int j = 0; j < Utils.GRID_SIZE; j++) {
				emptyPositions.Add (new Position (i, j));
			}
		}
	}

	public Position NextMove ()
	{
		var index = Random.Range (0, emptyPositions.Count - 1);
		Position pos = emptyPositions [index];
		emptyPositions.RemoveAt (index);
		return pos;
	}

}
