using UnityEngine;
using System.Collections;
using System.IO;

public interface IBattleSerializable
{
	
	void Serialize (BinaryWriter writer);

	void Deserialize (BinaryReader reader);

}
