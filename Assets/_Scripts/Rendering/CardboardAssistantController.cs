using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	public static CardboardAssistantController instance { get; private set; }

	bool VrMode;

	void Awake ()
	{
		if (Camera.main.GetComponent<StereoController> () != null) {
			(instance ?? this).ApplyCardboardSettings ();
		}
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	public void ApplyCardboardSettings ()
	{
		//Cardboard.SDK.TapIsTrigger = false; // causes OnBackButton to fire twice
		Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.On;
		Cardboard.SDK.EnableSettingsButton = true;
		Cardboard.SDK.OnBackButton += delegate {
			Prefs.VrMode = !Prefs.VrMode;
			Debug.Log ("BACK BUTTON!");
			SetvrModeAndUpdateCamera ();
		};
		Cardboard.SDK.VRModeEnabled = Prefs.VrMode;
#if UNITY_EDITOR
		Cardboard.SDK.StereoScreenScale = 2f;
#endif
	}

	void SetvrModeAndUpdateCamera ()
	{
		Cardboard.SDK.VRModeEnabled = Prefs.VrMode;
		Debug.Log ("***Cardboard.SDK.VRModeEnabled -> " + Cardboard.SDK.VRModeEnabled);
		Camera.main.GetComponent<StereoController> ().UpdateStereoValues ();
	}

}
