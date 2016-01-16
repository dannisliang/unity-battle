using UnityEngine;

public class PosRot
{

	public Vector3 position;
	public Quaternion rotation;

	public PosRot (Transform t)
	{
		this.position = t.position;
		this.rotation = t.rotation;
	}

	public PosRot (Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

}
