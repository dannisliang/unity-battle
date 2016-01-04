using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class FireAtWillController : MonoBehaviour
{
	bool firing;
	bool reticleAimingAtGrid;
	bool vrMode;

	Image image;
	Text text;

	void OnEnable ()
	{
		image = GetComponent<Image> ();
		text = GetComponentInChildren<Text> ();
		BattleshipController.instance.OnFiringStatus += UpdateFiring;
		BattleshipController.instance.OnReticleAimingAtGrid += UpdateAimAtGrid;
		Prefs.OnVrModeChanged += UpdateVrMode;
		UpdateText ();
	}

	void OnDisable ()
	{
		if (!GameController.instance.quitting) {
			BattleshipController.instance.OnFiringStatus -= UpdateFiring;
			BattleshipController.instance.OnReticleAimingAtGrid -= UpdateAimAtGrid;
			Prefs.OnVrModeChanged -= UpdateVrMode;
		}
	}

	void UpdateFiring (bool firing)
	{
		this.firing = firing;
		UpdateText ();
	}

	void UpdateAimAtGrid (bool reticleAimingAtGrid)
	{
		this.reticleAimingAtGrid = reticleAimingAtGrid;
		UpdateText ();
	}

	void UpdateVrMode (bool vrMode)
	{
		this.vrMode = vrMode;
		UpdateText ();
	}

	void UpdateText ()
	{
		bool show = reticleAimingAtGrid && !firing;
		image.enabled = show;
		text.enabled = show;
		if (show) {
			text.text = vrMode ?
			"Aim, then use\ntrigger to fire" :
			"Aim, then tap\nscreen to fire";
		}
	}

}
