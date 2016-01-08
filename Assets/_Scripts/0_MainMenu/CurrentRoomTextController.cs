using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentRoomTextController : MonoBehaviour
{

	public Button startGameButton;

	Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	void OnEnable ()
	{
		ButlerController.instance.OnConnectStatusChanged += UpdateStatus;
		ButlerController.instance.InvokeConnectStatusAction (UpdateStatus);
	}

	void OnDisable ()
	{
		ButlerController.instance.OnConnectStatusChanged -= UpdateStatus;
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
			int count = ButlerController.instance.GetConnectedParticipants ().Count;
			return "Starting " + count + " player game …";
		} 
		switch (roomSetupPercent) {
		case 0:
			return "Tap “" + startGameButton.GetComponentInChildren<Text> ().text + "” to play.";
		case 1:
			return "Setting up game …";
		case 20:
			return "Finding a suitable opponent …";
		default:
			return "Game is " + roomSetupPercent + "% ready …";
		}
	}
}
