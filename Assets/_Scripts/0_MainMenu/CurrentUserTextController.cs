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
			return "";
		case ConnectionStatus.AUTHENTICATED_NO_GAME:
		case ConnectionStatus.AUTHENTICATED_SETTING_UP_GAME:
		case ConnectionStatus.AUTHENTICATED_TEARING_DOWN_GAME:
		case ConnectionStatus.AUTHENTICATED_IN_GAME:
			return "Signed in " + Game.instance.GetLocalUsername ();
		default:
			throw new System.NotImplementedException ();
		}
	}
}
