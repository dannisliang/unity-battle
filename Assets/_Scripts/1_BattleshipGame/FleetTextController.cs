using UnityEngine;
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
		GetComponent<Text> ().text = boatPlacementController.grid == null ? "" : GetText (boatPlacementController.grid);
	}

	void OnEnable ()
	{
		boatPlacementController.grid.OnBoatHit += UpdateText;
		UpdateText ();
	}

	void OnDisable ()
	{
		boatPlacementController.grid.OnBoatHit += UpdateText;
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
