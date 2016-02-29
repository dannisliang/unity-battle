using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetTextController : MonoBehaviour
{
	public BoatPlacementController boatPlacementController;

	public Text text;

	void Start ()
	{
		BattleController.instance.OnBattleState += UpdateSelf;
	}

	void OnDisable ()
	{
		BattleController.instance.OnBattleState -= UpdateSelf;
	}

	void UpdateSelf (Whose whoseTurn, bool firing, Whose loser)
	{
		text.enabled = whoseTurn != Whose.Nobody;
		if (whoseTurn != Whose.Nobody) {
			text.text = GetText (boatPlacementController.grid);
		}
	}

	string GetText (Grid grid)
	{
		Boat[] boats = grid.boats;
		string t = "";
		if (boats != null) {
			for (int i = 0; i < boats.Length; i++) {
				BoatConfiguration config = boats [i].config;
				string color = boats [i].IsSunk () ? "#000" : "#999";
				t += config.designation + "\n<size=32><color=" + color + ">";
				for (int j = 0; j < boats [i].Size (); j++) {
					t += "▆ ";
				}
				t += "</color></size>";
				if (boats [i].IsSunk ()) {
					t += " <color='#f00'>(SUNK)</color>";
				}
				t += "\n\n";
			}
		}
		return t;
	}
}
