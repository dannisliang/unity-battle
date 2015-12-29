using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class VrModeToggleController : MonoBehaviour
{

	Toggle toggle;

	void Awake ()
	{
		toggle = GetComponent<Toggle> ();
		toggle.onValueChanged.AddListener (delegate {
			Cardboard.SDK.VRModeEnabled = toggle.isOn;
		});
	}

}
