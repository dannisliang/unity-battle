using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentGameTypeTextController : MonoBehaviour
{

	Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	void OnEnable ()
	{
		Game.instance.OnGameTypeChanged += UpdateStatus;
		Game.instance.InvokeGameTypeChanged (UpdateStatus);
	}

	void OnDisable ()
	{
		Game.instance.OnGameTypeChanged -= UpdateStatus;
	}

	void UpdateStatus (GameType gameType)
	{
		text.text = GetStatus (gameType);
	}

	string GetStatus (GameType gameType)
	{
		switch (gameType) {
		case GameType.NONE_SELECTED:
			return "";
		case GameType.ONE_PLAYER_DEMO:
			return "One Player Demo";
		case GameType.TWO_PLAYER_PLAY_GAMES:
			return "Two Player Online";
		default:
			throw new System.NotImplementedException ();
		}
	}
}
