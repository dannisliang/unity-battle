using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentUserTextController : MonoBehaviour
{

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
		return authenticated ? "Signed in " + ButlerController.instance.GetLocalUser ().userName.ToString () : "";
	}
}
