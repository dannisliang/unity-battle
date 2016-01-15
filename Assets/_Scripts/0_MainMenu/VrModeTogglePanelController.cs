using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class VrModeTogglePanelController : MonoBehaviour
{

	Animator animator;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
	}

	void Start ()
	{
		Prefs.OnVrModeChanged += MaybeTrigger;
		MaybeTrigger (Prefs.VrMode);
	}

	void OnDestroy ()
	{
		Prefs.OnVrModeChanged -= MaybeTrigger;
	}

	void MaybeTrigger (bool vrMode)
	{
		if (!vrMode) {
			animator.SetTrigger ("Show");
		}
	}

}
