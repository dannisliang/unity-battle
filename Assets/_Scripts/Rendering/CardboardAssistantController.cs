using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class CardboardAssistantController : MonoBehaviour
{
	public Camera gameCamera;
	public Vector3 vrCameraPosition;
	public Vector3 magicWindowCameraPosition;

	GameState gameState;
	bool gameVrMode;

	void Start ()
	{
		Cardboard.SDK.OnBackButton += BackButtonPressed;
		Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.Off;
//		Cardboard.SDK.ElectronicDisplayStabilization = false;
//		Cardboard.SDK.AutoDriftCorrection = false;
		Cardboard.SDK.EnableSettingsButton = true;

		Game.instance.OnGameStateChange += UpdateGameState;
	}

	void OnDestroy ()
	{
		Game.instance.OnGameStateChange -= UpdateGameState;
	}

	void BackButtonPressed ()
	{
		Debug.Log ("***BackButtonPressed()");
		switch (gameState) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.GAME_WAS_TORN_DOWN:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
//			Debug.Log ("***Application.Quit()");
//			Application.Quit ();
			break;
		case GameState.SELECTING_VIEW_MODE:
			Game.instance.QuitGame ();
			break;
		case GameState.PLAYING:
			Game.instance.SelectViewMode (null);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	void UpdateGameState (GameState state)
	{
		gameState = state;
		CheckVrMode ();
	}

	public void VrModeChanged (bool vrMode)
	{
		gameVrMode = vrMode;
		CheckVrMode ();
	}

	void CheckVrMode ()
	{
		bool vrMode = gameState == GameState.PLAYING ? gameVrMode : false;
		gameCamera.transform.position = vrMode ? vrCameraPosition : magicWindowCameraPosition;
		if (Cardboard.SDK.VRModeEnabled != vrMode) {
			Cardboard.SDK.VRModeEnabled = vrMode;
		}
	}

}
