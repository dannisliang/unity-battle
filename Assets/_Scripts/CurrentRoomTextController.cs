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
		text.text = PlayGamesPlatform.Instance.IsAuthenticated () && PlayGamesPlatform.Instance.RealTime.IsRoomConnected () ?
			"Room with " + PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants ().Count + " participants" : "(not in a room)";
	}
}
