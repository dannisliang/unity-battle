﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class QuickGameButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			GameController.instance.SetupRoom (false);
		});
	}

	void Start ()
	{
		GameController.OnConnectStatusChanged += UpdateActive;
		GameController.instance.InvokeConnectStatusAction (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!GameController.instance.quitting) {
			GameController.OnConnectStatusChanged -= UpdateActive;
		}
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated && roomSetupPercent == 0);
	}

}
