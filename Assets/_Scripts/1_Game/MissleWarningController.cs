using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CardboardAudioSource), typeof(Animator))]
public class MissleWarningController : MonoBehaviour
{
	[Range (0f, 1f)]
	public float volume;

	void OnDidApplyAnimationProperties ()
	{
		source.volume = volume;
	}

	CardboardAudioSource source;
	Animator animator;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
		source = GetComponent<CardboardAudioSource> ();
	}

	public void IssueWarning (float delay, float duration)
	{
		StartCoroutine (PlayFor (delay, duration));
	}

	IEnumerator PlayFor (float delay, float duration)
	{
		yield return new WaitForSeconds (delay);
		animator.SetTrigger ("MissleWarning");
		source.Play ();
		yield return new WaitForSeconds (duration);
	}
}
