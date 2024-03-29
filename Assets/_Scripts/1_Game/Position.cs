﻿using UnityEngine;
using System;
using UnityEngine.Assertions;
using System.IO;

public class Position : IBattleSerializable
{
	public int x { get; private set; }

	public int y { get; private set; }

	public Position ()
	{
	}

	public void Serialize (BinaryWriter writer)
	{	
		writer.Write (x);
		writer.Write (y);
	}

	public void Deserialize (BinaryReader reader)
	{
		x = reader.ReadInt32 ();
		y = reader.ReadInt32 ();
	}

	public Position (int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Position Above (int count)
	{
		return y < count ? null : new Position (x, y - count);
	}

	public Position Below (int count)
	{
		return y > Utils.GRID_SIZE.y - count - 1 ? null : new Position (x, y + count);
	}

	public Position Left (int count)
	{
		return x == count ? null : new Position (x - count, y);
	}

	public Position Right (int count)
	{
		return x > Utils.GRID_SIZE.x - count - 1 ? null : new Position (x + count, y);
	}

	public Vector3 AsGridLocalPosition (Marker marker)
	{
		return new Vector3 (x, Utils.GRID_SIZE.y - 1f - y, GetZ (marker));
	}

	float GetZ (Marker marker)
	{
		switch (marker) {
		case Marker.Aim:
		case Marker.Target:
			return -Utils.BOAT_HEIGHT - 2f * Utils.CLEARANCE_HEIGHT;
		case Marker.Hit:
			return -.5f * Utils.BOAT_HEIGHT - Utils.CLEARANCE_HEIGHT;
		case Marker.Miss:
			return -.5f * Utils.BOAT_HEIGHT - Utils.CLEARANCE_HEIGHT;
		default:
			throw new NotImplementedException ();
		}
	}

	public override bool Equals (System.Object obj)
	{
		if (obj == null) {
			return false;
		}
		Position other = (Position)obj;
		return other.x == x && other.y == y;
	}

	public override int GetHashCode ()
	{
		return x * 31 + y;
	}

	public override string ToString ()
	{
		return "" + ((char)(x + 65)) + (y + 1);
	}
}
