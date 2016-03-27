using UnityEngine;
using System.Collections;
using System;

public class GameTimer : MonoBehaviour
{
	static string CATEGORY = typeof(GameTimer).Name;

	GoogleAnalyticsV4 gav4;
	GameState previousGameState;
	float startTime;

	void Awake ()
	{
		startTime = Time.unscaledTime;
		gav4 = AnalyticsAssistant.gav4;
	}

	void OnEnable ()
	{
		Game.instance.OnGameStateChange += HandleGameStateChanged;
	}

	void HandleGameStateChanged (GameState state)
	{
		if (state == previousGameState) {
			return;
		}
		switch (previousGameState) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.SELECTING_VIEW_MODE:
			gav4.LogTiming (CATEGORY, Utils.DeltaTimeMillis (startTime), previousGameState.ToString () + "-" + state.ToString (), null);
			break;
		case GameState.PLAYING:
		case GameState.INITIALIZING_APP:
		case GameState.AUTHENTICATING:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
		case GameState.GAME_WAS_TORN_DOWN:
			break;
		default:
			throw new NotImplementedException ();
		}
		startTime = Time.unscaledTime;
		previousGameState = state;
	}

}
