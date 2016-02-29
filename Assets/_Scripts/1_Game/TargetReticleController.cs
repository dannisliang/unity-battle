using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class TargetReticleController : MonoBehaviour
{
	const float velocity = 8f;

	Animator animator;
	Vector3 targetPos;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
	}

	void Update ()
	{
		transform.localPosition = Vector3.Lerp (transform.localPosition, targetPos, Time.deltaTime * velocity);
	}

	public void SetTargetPosition (Position position, bool lockOnTarget)
	{
		gameObject.SetActive (position != null);
		if (position != null) {
			targetPos = position.AsGridLocalPosition (Marker.Target);
			if (lockOnTarget) {
				GetComponent<AudioSource> ().Play ();
				animator.SetTrigger ("LockOnTarget");
			}
		}
		GetComponent<Animator> ().enabled = lockOnTarget;
	}

}
