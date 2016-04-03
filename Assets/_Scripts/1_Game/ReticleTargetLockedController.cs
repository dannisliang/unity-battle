using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator), typeof(CardboardAudioSource))]
public class ReticleTargetLockedController : MonoBehaviour
{
	Animator animator;
	CardboardAudioSource audioSource;
	Vector3 targetPos;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
		audioSource = GetComponent<CardboardAudioSource> ();
		gameObject.SetActive (false);
	}

	public void SetPosition (Position position)
	{
		if (position != null) {
			transform.localPosition = position.AsGridLocalPosition (Marker.Target);
			audioSource.Play ();
			animator.SetTrigger ("LockOnTarget");
		}
		GetComponent<Animator> ().enabled = true;
	}

}
