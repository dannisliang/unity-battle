using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class GameStateHintController : MonoBehaviour
{
	public Color defaultBackgroundColor = Color.white;
	public Color syncingBackgroundColor = Color.blue;
	public Color identifyBackgroundColor = Color.white;
	
	bool playing;
	bool firing;
	bool reticleAimingAtGrid;
	bool vrMode;

	Image image;
	Text text;
	Boat reticleBoatTarget;

	void OnEnable ()
	{
		image = GetComponent<Image> ();
		text = GetComponentInChildren<Text> ();
		BattleshipController.instance.OnGameState += UpdateGameState;
		BattleshipController.instance.OnReticleAim += UpdateAimAtGrid;
		BattleshipController.instance.OnReticleIdentify += UpdateAimAtBoat;
		Prefs.OnVrModeChanged += UpdateVrMode;
		UpdateText ();
	}

	void OnDisable ()
	{
		if (!GameController.instance.quitting) {
			BattleshipController.instance.OnGameState -= UpdateGameState;
			BattleshipController.instance.OnReticleAim -= UpdateAimAtGrid;
			BattleshipController.instance.OnReticleIdentify -= UpdateAimAtBoat;
			Prefs.OnVrModeChanged -= UpdateVrMode;
		}
	}

	void UpdateGameState (bool playing, bool firing)
	{
		this.playing = playing;
		this.firing = firing;
		UpdateText ();
	}

	void UpdateAimAtGrid (Whose whose, Position position)
	{
		this.reticleAimingAtGrid = position != null;
		UpdateText ();
	}

	void UpdateAimAtBoat (Boat boat)
	{
		this.reticleBoatTarget = boat;
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
			return "Synchronizing.\nPlease wait …";
		}
		if (firing) {
			color = defaultBackgroundColor;
			return "Missle firing …";
		}
		if (reticleBoatTarget != null) {
			color = identifyBackgroundColor;
			return reticleBoatTarget.ToString ();
		}
		if (!reticleAimingAtGrid) {
			color = defaultBackgroundColor;
			return "Please locate the upper game grid;";
		}
		color = defaultBackgroundColor;
		return "Missle is armed and ready.\n" + (vrMode ?
			"Aim, then use trigger to fire." :
			"Aim, then tap screen to fire.");
	}

}
