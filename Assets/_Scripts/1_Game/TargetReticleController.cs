using UnityEngine;
using System.Collections;

public class TargetReticleController : MonoBehaviour
{

	public void SetTargetPosition (Position position)
	{
		gameObject.SetActive (position != null);
		if (position != null) {
			transform.localPosition = position.AsGridLocalPosition (Marker.Target);
		}
	}

}
