﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class StatsTextController : MonoBehaviour
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
		UpdateText ();
	}

	void OnDisable ()
	{
		boatPlacementController.grid.OnStrikeOccurred += UpdateText;
	}

	void UpdateText ()
	{
		text.text = GetText (boatPlacementController.grid);
	}

	string GetText (Grid grid)
	{
		Boat[] boats = grid.boats;
		string t = "";
		if (boats != null) {
			int units = 0;
			int sunk = 0;
			int hits = 0;
			int misses = grid.getMisses ();
			for (int i = 0; i < boats.Length; i++) {
				BoatConfiguration config = boats [i].config;
				units += config.size;
				hits += boats [i].HitCount ();
				if (boats [i].IsSunk ()) {
					sunk++;
				}
			}
			t += "Segments hit\n" + hits + " / " + units + "\n\n";
			t += "Boats sunk\n" + sunk + " / " + boats.Length + "\n\n";
			t += "Accuracy\n" + hits + " hits / " + misses + " misses\n\n";
			if (hits == units) {
				t += "\n<color='red'>YOU SUNK THE FLEET!!!</color>\n\n";
			}
		}
		return t;
	}
}