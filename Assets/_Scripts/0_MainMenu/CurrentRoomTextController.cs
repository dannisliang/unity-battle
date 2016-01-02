using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class CurrentRoomTextController : MonoBehaviour
{

	Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	void Start ()
	{
		UpdateStatus (GameController.instance.roomSetupPercent);
	}

	void OnEnable ()
	{
		GameController.OnRoomConnectStatusChanged += UpdateStatus;
	}

	void OnDisable ()
	{
		GameController.OnRoomConnectStatusChanged -= UpdateStatus;
	}

	void UpdateStatus (int percent)
	{
		text.text = GetStatus (percent);
	}

	string GetStatus (int percent)
	{
		if (GameController.gamesPlatform.IsAuthenticated () && GameController.gamesPlatform.RealTime.IsRoomConnected ()) {
			int count = GameController.gamesPlatform.RealTime.GetConnectedParticipants ().Count;
			return "Launching " + (count == 2 ? "two" : "" + count) + " player game…";
		} 
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
