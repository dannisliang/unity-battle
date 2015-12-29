using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class RealtimeBattleship : MonoBehaviour
{
	public static byte[] EncodeGrid (Grid grid)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			formatter.Serialize (stream, grid);
			byte[] bytes = stream.ToArray ();
			return bytes;
		}
	}

	public static Grid DecodeGrid (byte[] bytes)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream (bytes)) {
			Grid grid = formatter.Deserialize (stream) as Grid;
			return grid;
		}
	}

}
