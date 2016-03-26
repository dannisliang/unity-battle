using UnityEngine;
using System.Collections;
using System;

public class GameTimer : MonoBehaviour
{
	static string CATEGORY = typeof(GameTimer).Name;

	GoogleAnalyticsV4 gav4;
	float gameStartTime = 0f;

	void Awake ()
	{
		gav4 = AnalyticsAssistant.gav4;
	}

	void OnEnable ()
	{
		Game.instance.OnGameStateChange += HandleGameStateChanged;
	}

	void HandleGameStateChanged (GameState state)
	{
		switch (state) {
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.SELECTING_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.SELECTING_VIEW_MODE:
			break;
		case GameState.GAME_WAS_TORN_DOWN:
			MarkEndGame ();
			break;
		case GameState.PLAYING:
			MarkStartGame ();
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	void MarkStartGame ()
	{
		if (gameStartTime > 0f) {
			return;
		}
		gameStartTime = Time.unscaledTime;
	}

	void MarkEndGame ()
	{
		if (gameStartTime == 0f) {
			return;
		}
		gav4.LogTiming (CATEGORY, (long)(Time.unscaledTime - gameStartTime) * 1000L, "GamePlayTime", null);
		gameStartTime = 0f;
	}

}
