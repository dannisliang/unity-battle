using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class VrModeButtonController : MonoBehaviour
{

	Button toggle;

	void Awake ()
	{
		toggle = GetComponent<Button> ();
		toggle.onClick.AddListener (delegate {
			Prefs.VrMode = true;
			CardboardAssistantController.instance.ApplyCardboardSettings ();
		});
	}

}
