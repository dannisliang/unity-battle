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
		toggle.isOn = CardboardAssistantController.instance.VrMode;
		toggle.onValueChanged.AddListener (delegate {
			CardboardAssistantController.instance.VrMode = toggle.isOn;
		});
	}

}
