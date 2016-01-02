using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class FireAtWillTextController : MonoBehaviour
{

	void Start ()
	{
		UpdateText (Prefs.VrMode);
	}

	void OnEnable ()
	{
		Prefs.OnVrModeChanged += UpdateText;
	}

	void OnDisable ()
	{
		Prefs.OnVrModeChanged -= UpdateText;
	}

	void UpdateText (bool vrMode)
	{
		GetComponent<Text> ().text = vrMode ?
			"Aim, then use\ntrigger to fire" :
			"Aim, then tap\nscreen to fire";
	}

}
