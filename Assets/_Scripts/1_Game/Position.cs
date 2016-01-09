﻿using UnityEngine;

[System.Serializable]
public class Position
{
	public int x;
	public int y;

	public Position (int x, int y)
	{
		this.x = x;
		this.y = y;
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
