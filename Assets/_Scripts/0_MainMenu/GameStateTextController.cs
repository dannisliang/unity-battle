using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameStateTextController : MonoBehaviour
{

	Text text;

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
		text.text = GetText (state);
	}

	string GetText (GameState state)
	{
		switch (state) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.SELECTING_VIEW_MODE:
			return "";
		case GameState.AUTHENTICATING:
			return "Signing in …";
		case GameState.SETTING_UP_GAME:
			return "Seting up new game …";
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
