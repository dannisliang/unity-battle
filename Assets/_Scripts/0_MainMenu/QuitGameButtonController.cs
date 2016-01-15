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
		if (SceneMaster.quitting) {
			return;
		}
		Game.instance.OnGameStateChange -= UpdateActive;
	}

	void UpdateActive (GameState state)
	{
		switch (state) {
		case GameState.NEED_TO_SELECT_GAME_TYPE:
		case GameState.GAME_WAS_TORN_DOWN:
			gameObject.SetActive (false);
			break;
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.PLAYING:
			gameObject.SetActive (true);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

}
