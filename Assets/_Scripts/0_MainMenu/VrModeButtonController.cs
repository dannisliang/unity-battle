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

	void Start ()
	{
		Prefs.OnVrModeChanged += UpdateActive;
		UpdateActive (Prefs.VrMode);
	}

	void OnDestroy ()
	{
		Prefs.OnVrModeChanged -= UpdateActive;
	}

	void UpdateActive (bool vrMode)
	{
		gameObject.SetActive (!vrMode);
	}

	public void OnClick ()
	{
		Debug.Log ("***VR Mode Button CLICKED");
		Prefs.VrMode = !Prefs.VrMode;
	}

}
