﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaveRoomButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			ButlerController.instance.QuitGame ();
		});
	}

	void Start ()
	{
		ButlerController.instance.OnConnectStatusChanged += UpdateActive;
		ButlerController.instance.InvokeConnectStatusAction (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			ButlerController.instance.OnConnectStatusChanged -= UpdateActive;
		}
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated && roomSetupPercent > 0);
	}

}
