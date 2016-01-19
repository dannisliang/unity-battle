using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class QuitGameButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.butler.QuitGame ();
		});
	}

	void Start ()
	{
		Game.instance.OnGameStateChange += UpdateActive;
	}

	void OnDestroy ()
	{
		if (Game.instance != null) {
			Game.instance.OnGameStateChange -= UpdateActive;
		}
	}

	void UpdateActive (GameState state)
	{
		switch (state) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.GAME_WAS_TORN_DOWN:
			gameObject.SetActive (false);
			break;
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.SELECTING_VIEW_MODE:
		case GameState.PLAYING:
			gameObject.SetActive (true);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

}
