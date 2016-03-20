using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsTextController : MonoBehaviour
{
	public GridController boatPlacementController;

	public Text text;

	void OnEnable ()
	{
		BattleController.instance.OnBattleState += UpdateSelf;
	}

	void OnDisable ()
	{
		BattleController.instance.OnBattleState -= UpdateSelf;
	}

	void UpdateSelf (Whose whoseTurn, bool firing, Whose loser)
	{
		text.enabled = boatPlacementController.grid != null;
		if (text.enabled) {
			text.text = GetText (boatPlacementController.grid);
		}
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
