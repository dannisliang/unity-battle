using UnityEngine;
using UnityEngine.Assertions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class RealtimeBattle : MonoBehaviour
{

	const float AIM_INTERVAL = .5f;

	static float nextAimAllowed = 0f;
	static int lastAimMessageReceiveCount = 0;
	static Position unsentAimPosition;

	public static void EncodeAndSendGrid (Grid grid)
	{
		EncodeAndSend (Protocol.MessageType.GRID_POSITIONS, grid, true);
	}

	public static void EncodeAndSendLaunch (Position position)
	{
		EncodeAndSend (Protocol.MessageType.ROCKET_LAUNCH, position, true);
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
		EncodeAndSend (Protocol.MessageType.AIM_AT, position, false);
		nextAimAllowed = Time.unscaledTime + AIM_INTERVAL;
	}

	static void EncodeAndSend (Protocol.MessageType messageType, System.Object obj, bool reliable)
	{
		if (!Application.isEditor && messageType != Protocol.MessageType.AIM_AT) {
			Debug.Log ("***EncodeAndSend() [" + Game.butler + "]");
		}
		byte[] bytes = Protocol.Encode (messageType, obj, reliable);
		Game.instance.SendMessageToAll (reliable: reliable, data: bytes);
	}

	public static void DecodeAndExecute (byte[] bytes, bool reliable)
	{
		object obj;
		int messageCount;
		Protocol.MessageType messageType = Protocol.Decode (bytes, reliable, out obj, out messageCount);

		ExecuteDecodedMessage (obj, messageCount, messageType);
	}

	static void ExecuteDecodedMessage (object obj, int messageCount, Protocol.MessageType messageType)
	{
		switch (messageType) {
		case Protocol.MessageType.GRID_POSITIONS:
			Grid grid = obj as Grid;
			BattleController.instance.SetBoatsTheirs (grid.boats);
			break;
		case Protocol.MessageType.AIM_AT:
			if (messageCount < lastAimMessageReceiveCount) {
				Debug.LogWarning ("***Ignoring out of order uneliable message " + messageType + " #" + messageCount);
				return;
			}
			lastAimMessageReceiveCount = messageCount;
			Position aimPosition = obj as Position;
			BattleController.instance.AimAt (aimPosition);
			break;
		case Protocol.MessageType.ROCKET_LAUNCH:
			Position targetPosition = obj as Position;
			BattleController.instance.LaunchRocket (Whose.Ours, targetPosition, null);
			break;
		default:
			throw new NotImplementedException ("Unknown message type " + messageType);
		}
	}

}
