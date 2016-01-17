using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MagicWindowHintPanelController : MonoBehaviour
{

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

}
