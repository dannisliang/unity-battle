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
		gav4.LogTiming (CATEGORY, Utils.DeltaTimeMillis (startTime), previousGameState.ToString () + "-" + state.ToString (), null);
		startTime = Time.unscaledTime;
		previousGameState = state;
	}

}
