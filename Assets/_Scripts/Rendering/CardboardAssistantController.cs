using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	public static CardboardAssistantController instance { get; private set; }

	public bool VrMode;

	void Awake ()
	{
		if (instance != null && Camera.main.GetComponent<StereoController> () != null) {
			instance.ApplyCardboardSettings ();
		}
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	void ApplyCardboardSettings ()
	{
		string n = SceneManager.GetActiveScene ().name;
		Cardboard.SDK.TapIsTrigger = false;
		Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.On;
		Cardboard.SDK.EnableSettingsButton = true;
		Cardboard.SDK.OnBackButton += delegate {
			Debug.Log ("BACK BUTTON!");
		};
		Cardboard.SDK.VRModeEnabled = Prefs.VrMode;
#if UNITY_EDITOR
		Cardboard.SDK.StereoScreenScale = 2f;
#endif
	}

}
