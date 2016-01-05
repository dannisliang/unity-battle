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

	void OnEnable ()
	{
		GameController.OnConnectStatusChanged += UpdateStatus;
		GameController.instance.InvokeConnectStatusAction (UpdateStatus);
	}

	void OnDisable ()
	{
		GameController.OnConnectStatusChanged -= UpdateStatus;
	}

	void UpdateStatus (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		text.text = GetStatus (authenticated, isRoomConnected, roomSetupPercent);
	}

	string GetStatus (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		if (!authenticated) {
			return "Sign in required";
		}
		if (isRoomConnected) {
			int count = GameController.instance.GetConnectedParticipants ().Count;
			return "Launching " + (count == 2 ? "two" : "" + count) + " player game …";
		} 
		switch (roomSetupPercent) {
		case 0:
			return "Not in a game.\nWhy not join one?";
		case 1:
			return "Creating game…";
		case 20:
			return "Locating a suitable opponent…";
		default:
			return "Game is " + roomSetupPercent + "% ready…";
		}
	}
}
