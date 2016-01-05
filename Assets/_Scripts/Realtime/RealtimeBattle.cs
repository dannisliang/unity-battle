using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class RealtimeBattle : MonoBehaviour
{
	const byte MESSAGE_TYPE_GRID = (byte)'G';
	const byte MESSAGE_TYPE_SHOT = (byte)'S';

	public static void EncodeAndSend (Grid grid)
	{
		EncodeAndSend (MESSAGE_TYPE_GRID, grid);
	}

	public static void EncodeAndSend (Position position)
	{
		EncodeAndSend (MESSAGE_TYPE_SHOT, position);
	}

	static void EncodeAndSend (byte messageType, System.Object obj)
	{
		Debug.Log ("***EncodeAndSend() [authenticated==" + GameController.instance.authenticated + ", roomConnected==" + GameController.instance.roomConnected + ", roomSetupPercent=" + GameController.instance.roomSetupPercent + "]");

		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			stream.WriteByte (messageType);
			formatter.Serialize (stream, obj);
			byte[] bytes = stream.ToArray ();
			GameController.instance.SendMessageToAll (reliable: true, data: bytes);
		}
	}

	public static void DecodeAndExecute (byte[] bytes)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream (bytes)) {
			byte messageType = (byte)stream.ReadByte ();
			switch (messageType) {
			case MESSAGE_TYPE_GRID:
				Grid grid = formatter.Deserialize (stream) as Grid;
				Debug.Log ("***Received other grid ");// + grid);
				BattleController.instance.SetBoatsTheirs (grid.boats);
				break;
			case MESSAGE_TYPE_SHOT:
				Position position = formatter.Deserialize (stream) as Position;
				Debug.Log ("***Received shot at " + position);
				BattleController.instance.Strike (Whose.Ours, position);
				break;
			default:
				throw new System.NotImplementedException ("Unknown message type " + messageType);
			}
		}
	}

}
