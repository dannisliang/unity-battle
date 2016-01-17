﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class FleetTextController : MonoBehaviour
{
	public BoatPlacementController boatPlacementController;

	Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	void Start ()
	{
		GetComponent<Text> ().text = GetText (boatPlacementController.grid);
	}

	void OnEnable ()
	{
		boatPlacementController.grid.OnStrikeOccurred += UpdateText;
	}

	void OnDisable ()
	{
		boatPlacementController.grid.OnStrikeOccurred -= UpdateText;
	}

	void UpdateText (Whose whose, Boat boat, StrikeResult result)
	{
		text.text = GetText (boatPlacementController.grid);
	}

	string GetText (Grid grid)
	{
		Boat[] boats = grid.boats;
		string t = "";
		if (boats != null) {
			for (int i = 0; i < boats.Length; i++) {
				BoatConfiguration config = boats [i].config;
				t += config.designation + "\n- " + config.size + " units";
				if (boats [i].IsSunk ()) {
					t += " <color='#f00'>(SUNK)</color>";
				}
				t += "\n\n";
			}
		}
		return t;
	}
}
