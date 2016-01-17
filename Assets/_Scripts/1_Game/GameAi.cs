using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class GameAi :MonoBehaviour
{
	List<Position> emptyPositions;
	List<Position> unidentifiedHits;

	void Start ()
	{
		Init ();
	}

	void OnEnable ()
	{
		BattleController.instance.boatsOursPlacementController.grid.OnStrikeOccurred += NoteStrikeResult;
	}

	void OnDisable ()
	{
		BattleController.instance.boatsOursPlacementController.grid.OnStrikeOccurred -= NoteStrikeResult;
	}

	void Init ()
	{
		emptyPositions = new List<Position> ();
		for (int i = 0; i < Utils.GRID_SIZE; i++) {
			for (int j = 0; j < Utils.GRID_SIZE; j++) {
				emptyPositions.Add (new Position (i, j));
			}
		}
		unidentifiedHits = new List<Position> ();
	}

	void NoteStrikeResult (Whose whose, Boat boat, Position position, StrikeResult result)
	{
		switch (result) {
		case StrikeResult.MISS:
		case StrikeResult.IGNORED_ALREADY_MISSED:
		case StrikeResult.IGNORED_ALREADY_HIT:
			return;
		case StrikeResult.HIT_NOT_SUNK:
			unidentifiedHits.Add (position);
			return;
		case StrikeResult.HIT_AND_SUNK:
			for (int i = 0; i < boat.positions.Length; i++) {
				Position pos = boat.GetPosition (i);
				if (!pos.Equals (position)) {
					Assert.IsTrue (unidentifiedHits.Remove (pos));
				}
			}
			return;
		default:
			throw new System.NotImplementedException ();
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
