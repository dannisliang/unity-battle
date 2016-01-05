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

	public Color syncingBackgroundColor = Color.blue;
	public Color defaultBackgroundColor = Color.white;

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
		Color color;
		string t = GetText (out color);
		bool show = t != null;
		image.enabled = show;
		text.enabled = show;
		if (show) {
			text.text = t;
			image.color = color;
		}
	}

	string GetText (out Color color)
	{
		if (!playing) {
			color = syncingBackgroundColor;
			return "Synchronizing.\nPlease wait…";
		}
		if (firing | !reticleAimingAtGrid) {
			color = defaultBackgroundColor;
			return null;
		}
		color = defaultBackgroundColor;
		return "Missle is armed and ready.\n" + (vrMode ?
			"Aim, then use trigger to fire." :
			"Aim, then tap screen to fire.");
	}

}
