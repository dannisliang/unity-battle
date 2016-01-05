using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class CurrentUserTextController : MonoBehaviour
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
		return authenticated ? "Signed in " + GameController.instance.GetLocalUser ().userName.ToString () : "";
	}
}
