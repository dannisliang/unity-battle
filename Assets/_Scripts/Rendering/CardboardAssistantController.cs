using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	public static CardboardAssistantController instance { get; private set; }

	//	public Toggle cardboardToggle;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);

		Cardboard.SDK.TapIsTrigger = false;
//		cardboardToggle.isOn = Cardboard.SDK.VRModeEnabled;
#if UNITY_EDITOR
		Cardboard.SDK.StereoScreenScale = 2f;
#endif
	}

}
