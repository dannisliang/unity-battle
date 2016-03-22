﻿using UnityEngine;
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

	int vrModeFontSize = 6;
	int magicWindowFontSize = 16;

	class StrikeData
	{
		public Whose whose { get; private set; }

		public Boat boat{ get; private set; }

		public StrikeResult result{ get; private set; }

		public StrikeData (Whose whose, Boat boat, StrikeResult result)
		{
			this.whose = whose;
			this.boat = boat;
			this.result = result;
		}
	}

	Image image;
	Text text;

	GameState state;
	StrikeData strikeData;
	Whose whoseTurn;
	bool firing;
	Whose loser;
	bool reticleAimingAtGrid;
	bool vrMode;
	int fireCount;


	void Awake ()
	{
		image = GetComponent<Image> ();
		text = GetComponentInChildren<Text> ();
	}

	void OnEnable ()
	{
		strikeData = null;
		whoseTurn = Whose.Nobody;
		firing = false;
		loser = Whose.Nobody;
		reticleAimingAtGrid = false;
		fireCount = 0;
		
		Game.instance.OnGameStateChange += UpdateGameState;
		BattleController.instance.OnBattleState += UpdateBattleState;
		BattleController.instance.OnReticleAim += UpdateAimAtGrid;
		BattleController.instance.OnStrikeOccurred += UpdateStrikeOccurred;
		UpdateText ();
	}

	void OnDisable ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		Game.instance.OnGameStateChange -= UpdateGameState;
		BattleController.instance.OnBattleState -= UpdateBattleState;
		BattleController.instance.OnReticleAim -= UpdateAimAtGrid;
		BattleController.instance.OnStrikeOccurred -= UpdateStrikeOccurred;
	}

	void UpdateGameState (GameState state)
	{
//		Debug.Log ("UpdateGameState(state=" + state + ")");
		this.state = state;
		UpdateText ();
	}

	void UpdateBattleState (Whose whoseTurn, bool firing, Whose loser)
	{
//		Debug.Log ("UpdateBattleState(whoseTurn=" + whoseTurn + ", firing=" + firing + ", loser=" + loser + ")");
		this.whoseTurn = whoseTurn;
		this.firing = firing;
		this.loser = loser;
		if (firing) {
			fireCount++;
		}
		UpdateText ();
	}

	void UpdateAimAtGrid (Whose whose, Position position)
	{
		this.reticleAimingAtGrid = position != null;
		UpdateText ();
	}

	void UpdateStrikeOccurred (Whose whose, Boat boat, Position position, StrikeResult result)
	{
		SetStrikeData (whose, boat, result);
	}

	void SetStrikeData (Whose whose, Boat boat, StrikeResult result)
	{
		StrikeData newStrikeData = new StrikeData (whose, boat, result);
		this.strikeData = newStrikeData;
		UpdateText ();
		SceneMaster.instance.Async (delegate {
			if (this.strikeData == newStrikeData) {
				this.strikeData = null;
				UpdateText ();
			}
		}, GetTimeout (result));
	}

	float GetTimeout (StrikeResult result)
	{
		switch (result) {
		case StrikeResult.MISS:
			return 2f;
		case StrikeResult.IGNORED_ALREADY_MISSED:
		case StrikeResult.IGNORED_ALREADY_HIT:
			return 0f;
		case StrikeResult.HIT_NOT_SUNK:
		case StrikeResult.HIT_AND_SUNK:
			return 5f;
		default:
			throw new System.NotImplementedException ();
		}
	}

	void UpdateText ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		Color color;
		string t = GetText (out color);
//		Debug.Log ("***" + name + ".text => " + t);
		bool show = t != null;
		image.enabled = show;
		text.enabled = show;
		if (show) {
			text.text = t;
			text.fontSize = Cardboard.SDK.VRModeEnabled ? vrModeFontSize : magicWindowFontSize;
			image.color = color;
		}
	}

	string GetText (out Color color)
	{
		if (state != GameState.PLAYING) {
			color = defaultBackgroundColor;
			return null;
		}
		if (loser != Whose.Nobody) {
			color = loser == Whose.Ours ? theirColor : ourColor;
			return loser == Whose.Ours ?
				"Your entire fleet was sunk.\nYour opponent has won." :
				"You win! You sunk your\nopponent's entire fleet.";
		}
		if (whoseTurn == Whose.Nobody) {
			color = syncingBackgroundColor;
			return "Synchronizing.\nPlease wait …";
		}
		if (strikeData != null) {
			color = defaultBackgroundColor;
			switch (strikeData.result) {
			case StrikeResult.MISS:
			case StrikeResult.IGNORED_ALREADY_MISSED:
			case StrikeResult.IGNORED_ALREADY_HIT:
			case StrikeResult.HIT_NOT_SUNK:
				break;
			case StrikeResult.HIT_AND_SUNK:
				return (strikeData.whose == Whose.Theirs ?
					"You sunk your opponent's\n"
						: "Your opponent sunk your\n")
				+ strikeData.boat.config.designation + "!";
			default:
				throw new System.NotImplementedException ();
			}
		}
		if (firing) {
			color = defaultBackgroundColor;
			return null; // "Firing missle …";
		}
		if (!reticleAimingAtGrid) {
			color = defaultBackgroundColor;
			return null; // "Locate the upper game grid\nto target your opponent's ships."
		}
		color = defaultBackgroundColor;
		return GetReadyMessage ();
	}

	string GetReadyMessage ()
	{
		if (whoseTurn != Whose.Ours) {
			return null;
		}
		string longAimimText = Cardboard.SDK.VRModeEnabled ?
			"Aim, then use trigger to fire." :
			"Aim, then tap screen to fire.";
		switch (fireCount) {
		case 0:
			//return "Missle is armed and ready.\n" + longAimimText;
			return longAimimText;
		case 1:
			return longAimimText;
		case 2:
			return "Aim, then fire.";
		default:
			return null;
		}
	}
}
