using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class GameAi : MonoBehaviour
{
	object _lock = new System.Object ();

	List<Position> _emptyPositions;
	List<Position> _unidentifiedHits;

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
		lock (_lock) {
			_emptyPositions = new List<Position> ();
			for (int i = 0; i < Utils.GRID_SIZE; i++) {
				for (int j = 0; j < Utils.GRID_SIZE; j++) {
					_emptyPositions.Add (new Position (i, j));
				}
			}
			_unidentifiedHits = new List<Position> ();
		}
	}

	void NoteStrikeResult (Whose whose, Boat boat, Position position, StrikeResult result)
	{
		lock (_lock) {
			switch (result) {
			case StrikeResult.MISS:
			case StrikeResult.IGNORED_ALREADY_MISSED:
			case StrikeResult.IGNORED_ALREADY_HIT:
				return;
			case StrikeResult.HIT_NOT_SUNK:
				_unidentifiedHits.Add (position);
				return;
			case StrikeResult.HIT_AND_SUNK:
				for (int i = 0; i < boat.positions.Length; i++) {
					Position pos = boat.GetPosition (i);
					if (!pos.Equals (position)) {
						Assert.IsTrue (_unidentifiedHits.Remove (pos));
					}
				}
				return;
			default:
				throw new System.NotImplementedException ();
			}
		}
	}

	public Position NextMove ()
	{
		lock (_lock) {
			Position pos = AimInLine () ?? AimAround () ?? AimRandom ();
			_emptyPositions.Remove (pos);
			return pos;
		}
	}

	Position AimInLine ()
	{
		lock (_lock) {
			foreach (Position hitPos in _unidentifiedHits) {
				Position pos = NextInLine (hitPos);
				if (pos != null) {
					return pos;
				}
			}
			return null;
		}
	}

	Position NextInLine (Position hitPos)
	{
		lock (_lock) {
			return IfIsInLine (hitPos.Above (), hitPos.Above ().Above ())
			?? IfIsInLine (hitPos.Below (), hitPos.Below ().Below ())
			?? IfIsInLine (hitPos.Left (), hitPos.Left ().Left ())
			?? IfIsInLine (hitPos.Right (), hitPos.Right ().Right ());
		}
	}

	Position IfIsInLine (Position checkPosition, Position candidatePosition)
	{
		lock (_lock) {
			if (_unidentifiedHits.Contains (checkPosition) && _emptyPositions.Contains (candidatePosition)) {
				return candidatePosition;
			}
			return null;
		}
	}

	Position AimAround ()
	{
		lock (_lock) {
			foreach (Position hitPos in _unidentifiedHits) {
				Position pos = NextEmptyPositionAround (hitPos);
				if (pos != null) {
					return pos;
				}
			}
			return null;
		}
	}

	Position NextEmptyPositionAround (Position hitPos)
	{
		lock (_lock) {
			return IfIsOpenPosition (hitPos.Above ())
			?? IfIsOpenPosition (hitPos.Below ())
			?? IfIsOpenPosition (hitPos.Left ())
			?? IfIsOpenPosition (hitPos.Right ());
		}
	}

	Position IfIsOpenPosition (Position pos)
	{
		lock (_lock) {
			if (pos == null) {
				return null;
			}
			foreach (Position emptyPos in _emptyPositions) {
				if (pos.Equals (emptyPos)) {
					return pos;
				}
			}
			return null;
		}
	}

	public Position AimRandom ()
	{
		lock (_lock) {
			var index = Random.Range (0, _emptyPositions.Count - 1);
			Position pos = _emptyPositions [index];
			return pos;
		}
	}

}
