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
		Game.instance.OnConnectStatusChanged += UpdateStatus;
		Game.instance.InvokeConnectStatusAction (UpdateStatus);
	}

	void OnDisable ()
	{
		Game.instance.OnConnectStatusChanged -= UpdateStatus;
	}

	void UpdateStatus (ConnectionStatus status)
	{
		text.text = GetStatus (status);
	}

	string GetStatus (ConnectionStatus status)
	{
		switch (status) {
		case ConnectionStatus.AUTHENTICATION_REQUIRED:
			return "Sign in required";
		case ConnectionStatus.AUTHENTICATED_NO_GAME:
			return "Tap “" + startGameButton.GetComponentInChildren<Text> ().text + "” to play.";
		case ConnectionStatus.AUTHENTICATED_SETTING_UP_GAME:
			return "Finding a suitable opponent …";
		case ConnectionStatus.AUTHENTICATED_TEARING_DOWN_GAME:
			return "Quitting game …";
		case ConnectionStatus.AUTHENTICATED_IN_GAME:
			int count = Game.butler.GetConnectedParticipantCount ();
			return "Starting " + count + " player game …";
		default:
			throw new System.NotImplementedException ();
		}
	}
}
