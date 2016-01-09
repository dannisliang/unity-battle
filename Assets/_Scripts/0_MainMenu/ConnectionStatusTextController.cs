using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ConnectionStatusTextController : MonoBehaviour
{

	Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	void OnEnable ()
	{
		Game.instance.OnGameStateChange += UpdateText;
		Game.instance.InvokeConnectStatusAction (UpdateText);
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
		return state.ToString ();
//		switch (state) {
//		case GameState.SELECTING_GAME_TYPE:
//			return "";
//		case GameState.AUTHENTICATING:
//			return "";
//		case GameState.SETTING_UP_GAME:
//		case GameState.TEARING_DOWN_GAME:
//		case GameState.PLAYING:
//			return "Signed in " + Game.instance.GetLocalUsername ();
//		default:
//			throw new NotImplementedException ();
//		}
	}
}
