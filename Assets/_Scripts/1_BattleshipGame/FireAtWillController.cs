using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class FireAtWillController : MonoBehaviour
{

	public void SetVisible (bool active)
	{
		gameObject.SetActive (active);
	}

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
		GetComponentInChildren<Text> ().text = vrMode ?
			"Aim, then use\ntrigger to fire" :
			"Aim, then tap\nscreen to fire";
	}

}
