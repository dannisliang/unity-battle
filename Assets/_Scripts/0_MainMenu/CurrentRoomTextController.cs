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
		if (GameController.gamesPlatform.IsAuthenticated () && GameController.gamesPlatform.RealTime.IsRoomConnected ()) {
			text.text = "Room with " + GameController.gamesPlatform.RealTime.GetConnectedParticipants ().Count + " participants";
		} else {
			int percent = GameController.instance.RoomSetupPercent ();
			text.text = percent == 0 ? "(not in a room)" : "Setting up room (" + percent + "%)";
		}
	}
}
