using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Protocol
{
	public const int PROTOCOL_VERSION = 2;

	static int sendMessageCount = 0;

	public enum MessageType : byte
	{
		GRID_POSITIONS = (byte)'G',
		ROCKET_LAUNCH = (byte)'L',
		AIM_AT = (byte)'A',
	}

	public static byte[] Encode (MessageType messageType, System.Object obj, bool reliable)
	{
		Debug.Log ("***Encode() [" + Game.butler + "]");
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream ()) {
			stream.WriteByte ((byte)messageType);
			formatter.Serialize (stream, sendMessageCount);
			formatter.Serialize (stream, obj);
			sendMessageCount++;
			return stream.ToArray ();
		}
	}

	public static Protocol.MessageType Decode (byte[] bytes, bool reliable, out object obj, out int messageCount)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		using (MemoryStream stream = new MemoryStream (bytes)) {
			MessageType messageType = (MessageType)stream.ReadByte ();
			messageCount = formatter.Deserialize (stream) as int? ?? -1;
			switch (messageType) {
			case Protocol.MessageType.GRID_POSITIONS:
				obj = formatter.Deserialize (stream) as Grid;
				Debug.Log ("***Received other grid ");// + grid);
				break;
			case Protocol.MessageType.AIM_AT:
				obj = formatter.Deserialize (stream) as Position;
				Debug.Log ("***Received aim at " + obj);
				break;
			case Protocol.MessageType.ROCKET_LAUNCH:
				obj = formatter.Deserialize (stream) as Position;
				Debug.Log ("***Received launch at " + obj);
				break;
			default:
				throw new NotImplementedException ("Unknown message type " + messageType);
			}
			return messageType;
		}
	}

}
