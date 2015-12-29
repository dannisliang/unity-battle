using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class RealtimeBattleship : MonoBehaviour
{
	static byte MESSAGE_TYPE_GRID = (byte)'G';
	static byte MESSAGE_TYPE_SHOT = (byte)'S';

	public static byte[] EncodeGrid (Grid grid)
	{
		return Encode (MESSAGE_TYPE_GRID, grid);
	}

	public static byte[] EncodeGrid (Position position)
	{
		return Encode (MESSAGE_TYPE_SHOT, position);
	}

	public static byte[] Encode (byte messageType, System.Object obj)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			stream.WriteByte (messageType);
			formatter.Serialize (stream, obj);
			byte[] bytes = stream.ToArray ();
			return bytes;
		}
	}

	public static Grid DecodeGrid (byte[] bytes)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream (bytes)) {
			Assert.IsTrue (stream.ReadByte () == MESSAGE_TYPE_GRID);
			Grid grid = formatter.Deserialize (stream) as Grid;
			return grid;
		}
	}

}
