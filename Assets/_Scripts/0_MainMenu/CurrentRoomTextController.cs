using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class CurrentRoomTextController : MonoBehaviour
{

	Text text;

	void Start ()
	{
		text = GetComponent<Text> ();
	}

	void Update ()
	{
		text.text = GetStatus ();
	}

	string GetStatus ()
	{
		if (GameController.gamesPlatform.IsAuthenticated () && GameController.gamesPlatform.RealTime.IsRoomConnected ()) {
			int count = GameController.gamesPlatform.RealTime.GetConnectedParticipants ().Count;
			return "Launching " + (count == 2 ? "two" : "" + count) + " player game…";
		} 
		int percent = GameController.instance.RoomSetupPercent ();
		switch (percent) {
		case 0:
			return "Not in a game.\nWhy not join one?";
		case 1:
			return "Creating game…";
		case 20:
			return "Locating a suitable opponent…";
		default:
			return "Game is " + percent + "% ready…";
		}
	}
}
