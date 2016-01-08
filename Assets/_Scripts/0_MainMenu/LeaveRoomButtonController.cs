using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LeaveRoomButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.instance.QuitGame ();
		});
	}

	void Start ()
	{
		Game.instance.OnConnectStatusChanged += UpdateActive;
		Game.instance.InvokeConnectStatusAction (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			Game.instance.OnConnectStatusChanged -= UpdateActive;
		}
	}

	void UpdateActive (ConnectionStatus status)
	{
		switch (status) {
		case ConnectionStatus.GAME_TYPE_SELECTION_REQUIRED:
		case ConnectionStatus.AUTHENTICATION_REQUIRED:
		case ConnectionStatus.AUTHENTICATED_NO_GAME:
			gameObject.SetActive (false);
			break;
		case ConnectionStatus.AUTHENTICATED_SETTING_UP_GAME:
		case ConnectionStatus.AUTHENTICATED_TEARING_DOWN_GAME:
		case ConnectionStatus.AUTHENTICATED_IN_GAME:
			gameObject.SetActive (true);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

}
