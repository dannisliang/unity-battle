using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class WaitingForOpponentController : MonoBehaviour
{

	Animator animator;

	bool playing;
	bool firing;
	Whose? loser;
	Whose whoseTurn;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
	}

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
