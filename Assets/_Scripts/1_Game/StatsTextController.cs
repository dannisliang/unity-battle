using UnityEngine;
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
			t += "Segments hit: " + AsPercentage (hits, units) + "\n" + hits + " / " + units + "\n\n";
			t += "Boats sunk: " + AsPercentage (sunk, boats.Length) + "\n" + sunk + " / " + boats.Length + "\n\n";
			t += "Accuracy: " + AsPercentage (hits, hits + misses) + "\n" + hits + " / " + (hits + misses) + "\n\n";
		}
		return t;
	}

	string AsPercentage (float a, float b)
	{
		return b == 0 ? "" : (a / b).ToString ("P1").Replace (".0", "").Replace (" ", "");
	}
}
