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
		toggle.isOn = Prefs.VrMode;
		toggle.onValueChanged.AddListener (delegate {
			Prefs.VrMode = toggle.isOn;
		});
	}

}
