using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class RealtimeBattle : MonoBehaviour
{
	const float AIM_INTERVAL = 1f;

	static float nextAimAllowed = 0f;
	
	const byte MESSAGE_TYPE_GRID = (byte)'G';
	const byte MESSAGE_TYPE_SHOT = (byte)'S';
	const byte MESSAGE_TYPE_AIM = (byte)'A';

	public static void EncodeAndSendGrid (Grid grid)
	{
		EncodeAndSend (MESSAGE_TYPE_GRID, grid);
	}

	public static void EncodeAndSendHit (Position position)
	{
		EncodeAndSend (MESSAGE_TYPE_SHOT, position);
	}

	public static void MaybeEncodeAndSendAim (Position position)
	{
		if (Time.unscaledTime < nextAimAllowed) {
			return;
		}
		EncodeAndSend (MESSAGE_TYPE_AIM, position, reliable: false);
		nextAimAllowed = Time.unscaledTime + AIM_INTERVAL;
	}

	static void EncodeAndSend (byte messageType, System.Object obj, bool reliable = true)
	{
		Debug.Log ("***EncodeAndSend() [" + Game.butler + "]");
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			stream.WriteByte (messageType);
			formatter.Serialize (stream, obj);
			byte[] bytes = stream.ToArray ();
			Game.instance.SendMessageToAll (reliable: reliable, data: bytes);
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
			case MESSAGE_TYPE_AIM:
				Position aimPosition = formatter.Deserialize (stream) as Position;
				Debug.Log ("***Received aim at " + aimPosition);
				BattleController.instance.AimAt (aimPosition);
				break;
			case MESSAGE_TYPE_SHOT:
				Position shotPosition = formatter.Deserialize (stream) as Position;
				Debug.Log ("***Received shot at " + shotPosition);
				BattleController.instance.Strike (Whose.Ours, shotPosition);
				break;
			default:
				throw new NotImplementedException ("Unknown message type " + messageType);
			}
		}
	}

}
