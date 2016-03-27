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
		Debug.Log (CATEGORY + ": " + ((long)(Time.unscaledTime - startTime) * 1000L) + previousGameState.ToString () + "-" + state.ToString ());
		gav4.LogTiming (CATEGORY, (long)(Time.unscaledTime - startTime) * 1000L, previousGameState.ToString () + "-" + state.ToString (), null);
		startTime = Time.unscaledTime;
		previousGameState = state;
	}

}
