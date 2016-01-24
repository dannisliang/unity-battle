using UnityEngine;
using System.Collections;

public class TargetReticleController : MonoBehaviour
{

	public void SetTargetPosition (Position position, bool lockOnTarget)
	{
		gameObject.SetActive (position != null);
		if (position != null) {
			transform.localPosition = position.AsGridLocalPosition (Marker.Target);
			if (lockOnTarget) {
				GetComponent<AudioSource> ().Play ();
			}
		}
		GetComponent<Animator> ().enabled = lockOnTarget;
	}

}
