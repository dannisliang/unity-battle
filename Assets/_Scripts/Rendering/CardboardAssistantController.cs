using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	public Camera gameCamera;
	public Vector3 vrCameraPosition;
	public Vector3 magicWindowCameraPosition;

	GameState gameState;
	bool gameVrMode;

	void Start ()
	{
		Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.On;
//		Cardboard.SDK.ElectronicDisplayStabilization = false;
//		Cardboard.SDK.AutoDriftCorrection = false;
		Cardboard.SDK.EnableSettingsButton = true;
//		Cardboard.SDK.StereoScreenScale = Application.isEditor ? 1f : .8f;
		// just in case VrMode already set to default
//		Camera.main.GetComponent<StereoController> ().UpdateStereoValues ();

		Game.instance.OnGameStateChange += UpdateGameState;
	}

	void OnDestroy ()
	{
		Game.instance.OnGameStateChange -= UpdateGameState;
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
		if (Cardboard.SDK.VRModeEnabled == vrMode) {
			return;
		}
		Cardboard.SDK.VRModeEnabled = vrMode;
//		Camera.main.GetComponent<StereoController> ().UpdateStereoValues ();
		gameCamera.transform.position = (vrMode || Application.isEditor) ? vrCameraPosition : magicWindowCameraPosition;
	}

}
