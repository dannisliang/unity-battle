using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class QuitGameButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.instance.QuitGame ();
		});
	}

	void Start ()
	{
		Game.instance.OnGameStateChange += UpdateActive;
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			Game.instance.OnGameStateChange -= UpdateActive;
		}
	}

	void UpdateActive (GameState state)
	{
		switch (state) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
			gameObject.SetActive (false);
			break;
		case GameState.TEARING_DOWN_GAME:
		case GameState.PLAYING:
			gameObject.SetActive (true);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

}
