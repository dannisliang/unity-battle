using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class GameStateHintController : MonoBehaviour
{
	bool playing;
	bool firing;
	bool reticleAimingAtGrid;
	bool vrMode;

	Image image;
	Text text;

	void OnEnable ()
	{
		image = GetComponent<Image> ();
		text = GetComponentInChildren<Text> ();
		BattleshipController.instance.OnGameState += UpdateGameState;
		BattleshipController.instance.OnReticleAimingAtGrid += UpdateAimAtGrid;
		Prefs.OnVrModeChanged += UpdateVrMode;
		UpdateText ();
	}

	void OnDisable ()
	{
		if (!GameController.instance.quitting) {
			BattleshipController.instance.OnGameState -= UpdateGameState;
			BattleshipController.instance.OnReticleAimingAtGrid -= UpdateAimAtGrid;
			Prefs.OnVrModeChanged -= UpdateVrMode;
		}
	}

	void UpdateGameState (bool playing, bool firing)
	{
		this.playing = playing;
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
		string t = GetText ();
		bool show = t != null;
		image.enabled = show;
		text.enabled = show;
		if (show) {
			text.text = t;
		}
	}

	string GetText ()
	{
		if (!playing) {
			return "Synchronizing.\nPlease wait…";
		}
		if (firing | !reticleAimingAtGrid) {
			return null;
		}
		return "Missle is armed.\n" + (vrMode ?
			"Aim, then use\ntrigger to fire." :
			"Aim, then tap\nscreen to fire.");
	}

}
