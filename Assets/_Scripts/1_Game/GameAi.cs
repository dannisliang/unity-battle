using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class GameAi : MonoBehaviour
{
	object _lock = new System.Object ();

	List<Position> _remainingPositions;
	List<Position> _unidentifiedHits;
	List<Position> _identifiedHits;
	List<Position> _identifiedMiss;

	void OnEnable ()
	{
		BattleController.instance.OnStrikeOccurred += NoteStrikeResult;
		Init ();
	}

	void OnDisable ()
	{
		BattleController.instance.OnStrikeOccurred -= NoteStrikeResult;
	}

	void Init ()
	{
		lock (_lock) {
			_remainingPositions = new List<Position> ();
			for (int i = 0; i < Utils.GRID_SIZE.x; i++) {
				for (int j = 0; j < Utils.GRID_SIZE.y; j++) {
					_remainingPositions.Add (new Position (i, j));
				}
			}
			_unidentifiedHits = new List<Position> ();
			_identifiedHits = new List<Position> ();
			_identifiedMiss = new List<Position> ();
		}
	}

	public Position NextMove ()
	{
		lock (_lock) {
			Position pos = AimInLine () ?? AimAround () ?? AimRandom ();
			_remainingPositions.Remove (pos);
			return pos;
		}
	}

	void NoteStrikeResult (Whose whose, Boat boat, Position position, StrikeResult result)
	{
		if (whose == Whose.Theirs) {
			return;
		}
		lock (_lock) {
			switch (result) {
			case StrikeResult.MISS:
				_identifiedMiss.Add (position);
				break;
			case StrikeResult.HIT_NOT_SUNK:
				_unidentifiedHits.Add (position);
				break;
			case StrikeResult.HIT_AND_SUNK:
				for (int i = 0; i < boat.positions.Length; i++) {
					Position pos = boat.GetPosition (i);
					if (!pos.Equals (position)) {
						Assert.IsTrue (_unidentifiedHits.Remove (pos));
					}
					_identifiedHits.Add (pos);
				}
				break;
			case StrikeResult.IGNORED_ALREADY_MISSED:
			case StrikeResult.IGNORED_ALREADY_HIT:
			default:
				throw new System.NotImplementedException ();
			}
//			Debug.Log (TextGrid ());
		}
	}

	//	string TextGrid ()
	//	{
	//		string t = "";
	//		for (int j = 0; j < Utils.GRID_SIZE.y; j++) {
	//			t += "GRID: ";
	//			for (int i = 0; i < Utils.GRID_SIZE.x; i++) {
	//				t += Describe (new Position (i, j)) + " ";
	//			}
	//			t += "\n";
	//		}
	//		return t;
	//	}
	//
	//	string Describe (Position pos)
	//	{
	//		return (_remainingPositions.Contains (pos) ? "O" : "")
	//		+ (_unidentifiedHits.Contains (pos) ? "X" : "")
	//		+ (_identifiedHits.Contains (pos) ? "S" : "")
	//		+ (_identifiedMiss.Contains (pos) ? "-" : "");
	//	}

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
			return IfIsInLine (hitPos.Above (1), hitPos.Above (2))
			?? IfIsInLine (hitPos.Below (1), hitPos.Below (2))
			?? IfIsInLine (hitPos.Left (1), hitPos.Left (2))
			?? IfIsInLine (hitPos.Right (1), hitPos.Right (2));
		}
	}

	Position IfIsInLine (Position checkPosition, Position candidatePosition)
	{
		lock (_lock) {
			if (checkPosition == null || candidatePosition == null) {
				return null;
			}
			if (_unidentifiedHits.Contains (checkPosition) && _remainingPositions.Contains (candidatePosition)) {
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
			return IfIsOpenPosition (hitPos.Above (1))
			?? IfIsOpenPosition (hitPos.Below (1))
			?? IfIsOpenPosition (hitPos.Left (1))
			?? IfIsOpenPosition (hitPos.Right (1));
		}
	}

	Position IfIsOpenPosition (Position pos)
	{
		lock (_lock) {
			if (pos == null) {
				return null;
			}
			foreach (Position emptyPos in _remainingPositions) {
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
			if (_remainingPositions.Count == 0) {
				return new Position (0, 0);
			}
			var index = Random.Range (0, _remainingPositions.Count - 1);
			Position pos = _remainingPositions [index];
			return pos;
		}
	}

}
