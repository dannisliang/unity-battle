using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	void Start ()
	{
		Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.On;
		Cardboard.SDK.OnBackButton += OnBackButton;
		//		Cardboard.SDK.ElectronicDisplayStabilization = false;
		Cardboard.SDK.AutoDriftCorrection = false;
		Cardboard.SDK.EnableSettingsButton = true;
		Cardboard.SDK.StereoScreenScale = Application.isEditor ? 1f : .7f;
		VrModeChanged (Prefs.VrMode);
		// just in case VrMode already set to default
		Camera.main.GetComponent<StereoController> ().UpdateStereoValues ();
	}

	void OnEnable ()
	{
		Prefs.OnVrModeChanged += VrModeChanged;
	}

	void OnApplicationQuit ()
	{
		Cardboard.SDK.OnBackButton -= OnBackButton;
	}

	void OnDisable ()
	{
		Prefs.OnVrModeChanged -= VrModeChanged;
	}

	public void VrModeChanged (bool vrMode)
	{
		if (Cardboard.SDK.VRModeEnabled == vrMode) {
			return;
		}
		Cardboard.SDK.VRModeEnabled = vrMode;
		Camera.main.GetComponent<StereoController> ().UpdateStereoValues ();
	}

	void OnBackButton ()
	{
		Debug.Log ("***BACK BUTTON!");
		Prefs.VrMode = !Prefs.VrMode;
	}

}
