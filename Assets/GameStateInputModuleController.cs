using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent (typeof(GazeInputModule), typeof(StandaloneInputModule))]
public class GameStateInputModuleController : MonoBehaviour
{

	GazeInputModule gazeInputModule;
	StandaloneInputModule standaloneInputModule;

	void Awake ()
	{
		gazeInputModule = GetComponent<GazeInputModule> ();
		standaloneInputModule = GetComponent<StandaloneInputModule> ();
	}

	void Start ()
	{
		Game.instance.OnGameStateChange += UpdateActive;
	}

	void Destroy ()
	{
		if (Game.instance != null) {
			Game.instance.OnGameStateChange -= UpdateActive;
		}
	}

	void UpdateActive (GameState state)
	{
		if (state == GameState.PLAYING) {
			standaloneInputModule.enabled = false;
			gazeInputModule.enabled = true;
		} else {
			gazeInputModule.enabled = false;
			standaloneInputModule.enabled = true;
		}
	}

}
