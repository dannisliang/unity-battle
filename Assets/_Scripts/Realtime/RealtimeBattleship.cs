﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class RealtimeBattleship : MonoBehaviour
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
		bool IsAuthenticated = GameController.gamesPlatform.IsAuthenticated ();
		bool IsRoomConnected = IsAuthenticated && GameController.gamesPlatform.RealTime.IsRoomConnected ();
		Debug.Log ("***EncodeAndSend() [IsAuthenticated==" + IsAuthenticated + ", IsRoomConnected==" + IsRoomConnected + ", roomSetupPercent=" + GameController.instance.roomSetupPercent + "]");

		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			stream.WriteByte (messageType);
			formatter.Serialize (stream, obj);
			byte[] bytes = stream.ToArray ();
			GameController.gamesPlatform.RealTime.SendMessageToAll (true, bytes);
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
				Debug.Log ("***Received other grid " + grid);
				BattleshipController.instance.boatsTheirsPlacementController.SetGrid (grid);
				break;
			case MESSAGE_TYPE_SHOT:
				Position position = formatter.Deserialize (stream) as Position;
				Debug.Log ("***Received shot at " + position);
				BattleshipController.instance.Strike (Whose.Ours, position);
				break;
			default:
				throw new System.NotImplementedException ("Unknown message type " + messageType);
			}
		}
	}

}
