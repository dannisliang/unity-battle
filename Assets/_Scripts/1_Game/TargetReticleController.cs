using UnityEngine;
using System.Collections;

public class TargetReticleController : MonoBehaviour
{
	const float velocity = 8f;

	Vector3 targetPos;

	void Update ()
	{
		if (targetPos == null) {
			return;
		}
		transform.localPosition = Vector3.Lerp (transform.localPosition, targetPos, Time.deltaTime * velocity);
	}

	public void SetTargetPosition (Position position, bool lockOnTarget)
	{
		gameObject.SetActive (position != null);
		if (position != null) {
			targetPos = position.AsGridLocalPosition (Marker.Target);
			if (lockOnTarget) {
				GetComponent<AudioSource> ().Play ();
			}
		}
		GetComponent<Animator> ().enabled = lockOnTarget;
	}

}
