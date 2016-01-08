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
		GameController.instance.OnConnectStatusChanged += UpdateStatus;
		GameController.instance.InvokeConnectStatusAction (UpdateStatus);
	}

	void OnDisable ()
	{
		GameController.instance.OnConnectStatusChanged -= UpdateStatus;
	}

	void UpdateStatus (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		text.text = GetStatus (authenticated, isRoomConnected, roomSetupPercent);
	}

	string GetStatus (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		return authenticated ? "Signed in " + GameController.instance.GetLocalUsername () : "";
	}
}
