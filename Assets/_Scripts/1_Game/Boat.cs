using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

public class Boat : IBattleSerializable
{
	public Whose whose;

	public bool horizontal{ get; private set; }

	public Position[] positions{ get; private set; }

	public int boatConfigurationIndex { get; private set; }

	int[] hits;

	public BoatConfiguration config { get; private set; }

	public Boat ()
	{
	}

	public static Boat RandomBoat (Whose whose, int boatConfigurationIndex)
	{
		Boat boat = new Boat ();
		boat.boatConfigurationIndex = boatConfigurationIndex;
		boat.config = Grid.fleet [boatConfigurationIndex];
		boat.horizontal = boat.config.size > Utils.GRID_SIZE.y || Random.value > .5f;
		int u = Random.Range (0, (boat.horizontal ? Utils.GRID_SIZE.x : Utils.GRID_SIZE.y) - boat.config.size + 1);
		int v = Random.Range (0, boat.horizontal ? Utils.GRID_SIZE.y : Utils.GRID_SIZE.x);

		boat.positions = MakeBoatPositions (u, v, boat.config.size, boat.horizontal);
		boat.hits = new int[boat.config.size];
		return boat;
	}

	public void Serialize (BinaryWriter writer)
	{
		writer.Write ((int)whose);
		writer.Write (boatConfigurationIndex);
		writer.Write (horizontal);
		positions [0].Serialize (writer);
	}

	public void Deserialize (BinaryReader reader)
	{	
		whose = (Whose)reader.ReadInt32 ();

		boatConfigurationIndex = reader.ReadInt32 ();
		config = Grid.fleet [boatConfigurationIndex];
		hits = new int[config.size];

		horizontal = reader.ReadBoolean ();
		Position position = new Position ();
		position.Deserialize (reader);

		positions = MakeBoatPositions (horizontal ? position.x : position.y, horizontal ? position.y : position.x, config.size, horizontal);
	}

	public StrikeResult FireAt (Position position, bool testOnly = false)
	{
		for (int i = 0; i < config.size; i++) {
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
		return HitCount () == config.size;
	}

	public int HitCount ()
	{
		int count = 0;
		for (int i = 0; i < config.size; i++) {
			if (hits [i] > 0) {
				count++;
			}
		}
		return count;
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
		return (whose == Whose.Theirs ? "This is your opponent's\n" : "This is your\n")
		+ config.designation
		+ (IsSunk () ? " — SUNK." : ".");// + " — " + positions [0] + " " + (horizontal ? "Horizontal" : "Vertical");
	}

}
