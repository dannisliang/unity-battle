using UnityEngine;
using System.Collections;

public class CardboardRenderTextureWorkaround : MonoBehaviour
{

	#if UNITY_EDITOR

	int width, height;

	void Awake ()
	{
		if (!Application.isEditor) {
			Destroy (gameObject);
		}
	}

	void Update ()
	{
		// Workaround StereoController.keepStereoUpdated failing to
		// update render texture size when game view is resized in editor
		if (Screen.width != width || Screen.height != height) {
			ForceNewRenderTexture (CardboardProfile.DeviceTypes.GoggleTechC1Glass);
			ForceNewRenderTexture (CardboardProfile.DeviceTypes.CardboardMay2015);
			width = Screen.width;
			height = Screen.height;
		}
	}

	void ForceNewRenderTexture (CardboardProfile.DeviceTypes deviceType)
	{
		foreach (Camera camera in Camera.main.GetComponent<StereoController> ().GetComponentsInChildren<Camera>()) {
			camera.targetTexture = null;
		}
		if (Cardboard.SDK.StereoScreen != null) {
			Cardboard.SDK.StereoScreen.Release ();
		}
		Cardboard.SDK.StereoScreen = null;
		Cardboard.SDK.DeviceType = deviceType;
	}

	#endif
}
