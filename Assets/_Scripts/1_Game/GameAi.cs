using UnityEngine;
using System.Collections.Generic;

public class GameAi :MonoBehaviour
{
	List<Position> emptyPositions;

	void Start ()
	{
		SetupGrid ();
	}

	void OnEnable ()
	{
		BattleController.instance.boatsOursPlacementController.grid.OnStrikeOccurred += NoteStrikeResult;
	}

	void OnDisable ()
	{
		BattleController.instance.boatsOursPlacementController.grid.OnStrikeOccurred -= NoteStrikeResult;
	}

	void SetupGrid ()
	{
		emptyPositions = new List<Position> ();
		for (int i = 0; i < Utils.GRID_SIZE; i++) {
			for (int j = 0; j < Utils.GRID_SIZE; j++) {
				emptyPositions.Add (new Position (i, j));
			}
		}
	}

	void NoteStrikeResult (Whose whose, Boat boat, Position position, StrikeResult result)
	{
		Debug.Log ("***" + whose + " " + position + " is a " + result);
	}

	public Position NextMove ()
	{
		var index = Random.Range (0, emptyPositions.Count - 1);
		Position pos = emptyPositions [index];
		emptyPositions.RemoveAt (index);
		return pos;
	}

}
