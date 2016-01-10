using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class RealtimeBattle : MonoBehaviour
{
	const float AIM_INTERVAL = .5f;

	static float nextAimAllowed = 0f;

	static int lastAimMessageSendCount = 0;
	static int lastAimMessageReceiveCount = 0;
	static Position unsentAimPosition;
	
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

	public static void EncodeAndSendAim (Position position)
	{
		if (Time.unscaledTime < nextAimAllowed) {
			unsentAimPosition = position;
			SceneMaster.instance.Async (delegate {
				if (unsentAimPosition != null) {
					EncodeAndSendAim (unsentAimPosition);
				}
			}, AIM_INTERVAL);
			return;
		}
		unsentAimPosition = null;
		EncodeAndSend (MESSAGE_TYPE_AIM, position, messageCount: lastAimMessageSendCount++);
		nextAimAllowed = Time.unscaledTime + AIM_INTERVAL;
	}

	static void EncodeAndSend (byte messageType, System.Object obj, int messageCount = 0)
	{
		Debug.Log ("***EncodeAndSend() [" + Game.butler + "]");
		bool reliable = messageCount == 0;
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			stream.WriteByte (messageType);
			if (!reliable) {
				formatter.Serialize (stream, messageCount);
			}
			formatter.Serialize (stream, obj);
			byte[] bytes = stream.ToArray ();
			Game.instance.SendMessageToAll (reliable: reliable, data: bytes);
		}
	}

	public static void DecodeAndExecute (byte[] bytes, bool reliable)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream (bytes)) {
			byte messageType = (byte)stream.ReadByte ();
			if (!reliable) {
				int messageCount = formatter.Deserialize (stream) as int? ?? 0;
				if (messageCount < lastAimMessageReceiveCount) {
					Debug.LogWarning ("***Ignoring out of order uneliable message " + messageType + " #" + messageCount);
					return;
				}
				lastAimMessageReceiveCount = messageCount;
			}
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
