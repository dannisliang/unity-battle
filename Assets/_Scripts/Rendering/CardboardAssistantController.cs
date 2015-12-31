using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	//	public Toggle cardboardToggle;

	void Awake ()
	{
		Cardboard.SDK.TapIsTrigger = false;
//		cardboardToggle.isOn = Cardboard.SDK.VRModeEnabled;
#if UNITY_EDITOR
		Cardboard.SDK.StereoScreenScale = 2f;
#endif
	}

}
