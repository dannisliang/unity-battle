using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class CardboardAssistantController : MonoBehaviour
{
	const float pinZoomSpeed = 0.2f;

	public Camera gameCamera;

	float initialGameCameraZ;
	GameState gameState;
	bool gameVrMode;

	void Start ()
	{
		Cardboard.SDK.OnBackButton += BackButtonPressed;
		Cardboard.SDK.ElectronicDisplayStabilization = false;
		Cardboard.SDK.AutoDriftCorrection = false;
		Cardboard.SDK.EnableSettingsButton = true;

		Game.instance.OnGameStateChange += UpdateGameState;

		initialGameCameraZ = gameCamera.transform.position.z;
	}

	void Update ()
	{
		#if UNITY_EDITOR || !UNITY_ANDROID
		if (Input.GetKeyDown (KeyCode.R)) {
			Recenter ();
		}
		#endif
		if (!gameVrMode && Input.touchCount == 2) {
			HandlePinch ();
		}
	}

	void HandlePinch ()
	{
		Touch touch0 = Input.GetTouch (0);
		Touch touch1 = Input.GetTouch (1);

		Vector2 touchZeroPrevPos = touch0.position - touch0.deltaPosition;
		Vector2 touchOnePrevPos = touch1.position - touch1.deltaPosition;

		float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		float touchDeltaMag = (touch0.position - touch1.position).magnitude;
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
		SetGameCameraZ (Mathf.Clamp (gameCamera.transform.position.z - deltaMagnitudeDiff * pinZoomSpeed, -15, 0));
	}

	void SetGameCameraZ (float z)
	{
		Vector3 pos = gameCamera.transform.position;
		pos.z = z;
		gameCamera.transform.position = pos;
	}

	void OnDestroy ()
	{
		Game.instance.OnGameStateChange -= UpdateGameState;
	}

	public void Recenter ()
	{
		Cardboard.SDK.Recenter ();
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
			Game.instance.QuitGame ("BackButtonPressed");
			break;
		case GameState.PLAYING:
			Game.instance.SelectViewMode (null);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	void UpdateGameState (GameState gameState)
	{
		this.gameState = gameState;
		switch (gameState) {
		case GameState.SELECTING_GAME_TYPE:
		case GameState.AUTHENTICATING:
		case GameState.GAME_WAS_TORN_DOWN:
		case GameState.SETTING_UP_GAME:
		case GameState.TEARING_DOWN_GAME:
			Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.Off;
			break;
		case GameState.SELECTING_VIEW_MODE:
		case GameState.PLAYING:
			Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.On;
			break;
		default:
			throw new NotImplementedException ();
		}

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
		if (Cardboard.SDK.VRModeEnabled != vrMode) {
			Cardboard.SDK.VRModeEnabled = vrMode;
			SetGameCameraZ (initialGameCameraZ);
		}
	}

}
