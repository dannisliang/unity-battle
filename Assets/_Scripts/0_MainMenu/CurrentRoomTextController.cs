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
		text.text = GameController.gamesPlatform.IsAuthenticated () && GameController.gamesPlatform.RealTime.IsRoomConnected () ?
			"Room with " + GameController.gamesPlatform.RealTime.GetConnectedParticipants ().Count + " participants" : "(not in a room)";
	}
}
