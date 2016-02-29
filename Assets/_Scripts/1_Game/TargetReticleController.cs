using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator), typeof(CardboardAudioSource))]
public class TargetReticleController : MonoBehaviour
{
	const float velocity = 8f;

	Animator animator;
	CardboardAudioSource audioSource;
	Vector3 targetPos;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
		audioSource = GetComponent<CardboardAudioSource> ();
		gameObject.SetActive (false);
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
				audioSource.Play ();
				animator.SetTrigger ("LockOnTarget");
			}
		}
		GetComponent<Animator> ().enabled = lockOnTarget;
	}

}
