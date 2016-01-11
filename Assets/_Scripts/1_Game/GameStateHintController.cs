using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class GameStateHintController : MonoBehaviour
{
	public Color defaultBackgroundColor = Color.white;
	public Color syncingBackgroundColor = Color.blue;
	public Color identifyBackgroundColor = Color.white;
	public Color ourColor = new Color (.91f, .55f, .22f, .65f);
	public Color theirColor = new Color (.27f, .64f, 1f, .65f);
	
	bool playing;
	bool firing;
	Whose? loser;
	bool reticleAimingAtGrid;
	bool vrMode;

	Image image;
	Text text;
	Boat reticleBoatTarget;

	void OnEnable ()
	{
		image = GetComponent<Image> ();
		text = GetComponentInChildren<Text> ();
		BattleController.instance.OnGameState += UpdateGameState;
		BattleController.instance.OnReticleAim += UpdateAimAtGrid;
		BattleController.instance.OnReticleIdentify += UpdateAimAtBoat;
		Prefs.OnVrModeChanged += UpdateVrMode;
		UpdateText ();
	}

	void OnDisable ()
	{
		if (!SceneMaster.quitting) {
			BattleController.instance.OnGameState -= UpdateGameState;
			BattleController.instance.OnReticleAim -= UpdateAimAtGrid;
			BattleController.instance.OnReticleIdentify -= UpdateAimAtBoat;
			Prefs.OnVrModeChanged -= UpdateVrMode;
		}
	}

	void UpdateGameState (bool playing, bool firing, Whose? loser)
	{
		this.playing = playing;
		this.firing = firing;
		this.loser = loser;
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
		if (loser != null) {
			color = loser == Whose.Ours ? theirColor : ourColor;
			return loser == Whose.Ours ?
				"Your entire fleet was sunk.\nYour opponent has won." :
				"You win! You sunk your\nopponent's entire fleet.";
		}
		if (!playing) {
			color = syncingBackgroundColor;
			return "Synchronizing.\nPlease wait …";
		}
		if (firing) {
			color = defaultBackgroundColor;
			return "Firing missle …";
		}
		if (reticleBoatTarget != null) {
			color = identifyBackgroundColor;
			return reticleBoatTarget.ToString ();
		}
		if (!reticleAimingAtGrid) {
			color = defaultBackgroundColor;
			return "Locate the upper game grid\n" +
			"to target your opponent's ships.";
		}
		color = defaultBackgroundColor;
		return "Missle is armed and ready.\n" + (vrMode ?
			"Aim, then use trigger to fire." :
			"Aim, then tap screen to fire.");
	}

}
