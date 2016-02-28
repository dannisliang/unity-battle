using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CardboardAudioSource), typeof(Animator))]
public class CenterPanelController : MonoBehaviour
{
	[Range (0f, 1f)]
	public float volume;

	bool playing;
	bool firing;
	Whose? loser;
	Whose whoseTurn;

	void OnDidApplyAnimationProperties ()
	{
		MatchVolume ();
	}

	CardboardAudioSource source;
	Animator animator;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
		source = GetComponent<CardboardAudioSource> ();
		MatchVolume ();
	}

	void MatchVolume ()
	{
		source.volume = volume;
	}

	public void IssueWarning (float delay, float duration)
	{
		animator.SetBool ("WaitingForOpponent", false);
		animator.SetTrigger ("MissleWarning");
//		source.Play ();
//		StartCoroutine (PlayFor (delay, duration));
	}

	//	IEnumerator PlayFor (float delay, float duration)
	//	{
	//		yield return new WaitForSeconds (delay);
	//		animator.SetBool ("WaitingForOpponent", false);
	//		animator.SetTrigger ("MissleWarning");
	//		source.Play ();
	//		yield return new WaitForSeconds (duration);
	//	}

	void OnEnable ()
	{
		BattleController.instance.OnBattleState += UpdateBattleState;
		BattleController.instance.OnWhoseTurn += UpdateWhoseTurn;
	}

	void OnDisable ()
	{
		BattleController.instance.OnBattleState -= UpdateBattleState;
		BattleController.instance.OnWhoseTurn -= UpdateWhoseTurn;
	}

	void UpdateBattleState (bool playing, bool firing, Whose? loser)
	{
		this.playing = playing;
		this.firing = firing;
		this.loser = loser;
		UpdateWaitingForOpponent ();
	}

	void UpdateWhoseTurn (Whose whoseTurn)
	{
		this.whoseTurn = whoseTurn;
		UpdateWaitingForOpponent ();
	}

	void UpdateWaitingForOpponent ()
	{
		animator.SetBool ("WaitingForOpponent", playing && !firing && loser == null && whoseTurn == Whose.Theirs);
	}

}
