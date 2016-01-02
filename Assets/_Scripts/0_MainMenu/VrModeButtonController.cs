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
		toggle.onClick.AddListener (OnClick);
	}

	public void OnClick ()
	{
		Debug.Log ("***VR Mode Button CLICKED");
		Prefs.VrMode = !Prefs.VrMode;
	}

}
