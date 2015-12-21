using UnityEngine;

[System.Serializable]
public class Position
{
	public int x;
	public int z;

	public Position (int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	public override bool Equals (System.Object obj)
	{
		if (obj == null) {
			return false;
		}
		Position other = (Position)obj;
		return other.x == x && other.z == z;
	}

	public override int GetHashCode ()
	{
		return x * 31 + z;
	}

	public override string ToString ()
	{
		return "" + ((char)(x + 65)) + (z + 1);
	}
}
