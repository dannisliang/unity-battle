using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameStateTextController : MonoBehaviour
{

	Text text;

	GameState state;
	string failureReasonText;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	void OnEnable ()
	{
		Game.instance.OnGameStateChange += UpdateText;
	}

	void OnDisable ()
	{
		Game.instance.OnGameStateChange -= UpdateText;
	}

	void UpdateText (GameState state)
	{
		this.state = state;
		text.text = GetText (state);
	}

	public void SetFailureReasonText (string failureReasonText)
	{
		StartCoroutine (ClearFailureReasonText (failureReasonText, 2.5f));
	}

	IEnumerator ClearFailureReasonText (string failureReasonText, float delay)
	{
		this.failureReasonText = failureReasonText;
		UpdateText (state);
		yield return new WaitForSeconds (delay);
		this.failureReasonText = null;
		UpdateText (state);
	}

	string GetText (GameState state)
	{
		if (failureReasonText != null) {
			return failureReasonText;
		}
		switch (state) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.SELECTING_VIEW_MODE:
			return "";
		case GameState.AUTHENTICATING:
			return "Signing in …";
		case GameState.SETTING_UP_GAME:
			return "Waiting for your friend …";
		case GameState.TEARING_DOWN_GAME:
			return "Game is ending.";
		case GameState.GAME_WAS_TORN_DOWN:
			return "Game has ended.";
		case GameState.PLAYING:
			return "Game is ready.";
		default:
			return state.ToString ();
		}
	}
}
