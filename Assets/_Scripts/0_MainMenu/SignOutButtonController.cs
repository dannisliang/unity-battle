﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignOutButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.instance.SignOut ();
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

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated);
	}

}
