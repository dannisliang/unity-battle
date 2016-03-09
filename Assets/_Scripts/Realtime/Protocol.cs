using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Protocol
{
	public const int PROTOCOL_VERSION = 4;

	static int sendMessageCount = 0;

	public enum MessageType : byte
	{
		GRID_POSITIONS = (byte)'G',
		ROCKET_LAUNCH = (byte)'L',
		AIM_AT = (byte)'A',
	}

	public static byte[] Encode (MessageType messageType, IBattleSerializable obj, bool reliable)
	{
		if (!Application.isEditor && messageType != MessageType.AIM_AT) {
			Debug.Log ("***Encode(" + messageType + ")");
		}
		using (MemoryStream stream = new MemoryStream ()) {
			using (BinaryWriter writer = new BinaryWriter (stream)) {
				writer.Write ((byte)messageType);
				writer.Write (sendMessageCount);
				obj.Serialize (writer);
				sendMessageCount++;
				return stream.ToArray ();
			}
		}
	}

	public static MessageType GetMessageType (ref byte[] bytes)
	{
		return (MessageType)bytes [0];
	}

	public static MessageType Decode (ref byte[] bytes, bool reliable, out IBattleSerializable obj, out int messageCount)
	{
		using (MemoryStream stream = new MemoryStream (bytes)) {
			using (BinaryReader reader = new BinaryReader (stream)) {
				MessageType messageType = (MessageType)stream.ReadByte ();
				messageCount = reader.ReadInt32 ();
				switch (messageType) {
				case Protocol.MessageType.GRID_POSITIONS:
					obj = new Grid ();
					obj.Deserialize (reader);
//					Debug.Log ("***Received other grid ");// + obj);
					break;
				case Protocol.MessageType.AIM_AT:
					obj = new Position ();
					obj.Deserialize (reader);
//					Debug.Log ("***Received aim at " + obj);
					break;
				case Protocol.MessageType.ROCKET_LAUNCH:
					obj = new Position ();
					obj.Deserialize (reader);
//					Debug.Log ("***Received launch at " + obj);
					break;
				default:
					throw new NotImplementedException ("Unknown message type " + messageType);
				}
				return messageType;
			}
		}
	}

}
