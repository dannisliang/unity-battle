﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	// workaround SDK bug caused by calling 'Cardboard.SDK.OnBackButton -= …'
	// in OnDisable(), which may be cardboard instance is destroy
	bool instanceCreated;

	void Awake ()
	{
		Debug.Log ("***CardboardAssistantController.Awake()");
	}

	void Start ()
	{
		Debug.Log ("***CardboardAssistantController.Start()");
		instanceCreated = true;
		Cardboard.SDK.TapIsTrigger = false;
		Cardboard.SDK.BackButtonMode = Cardboard.BackButtonModes.On;
		Cardboard.SDK.OnBackButton += OnBackButton;
		//		Cardboard.SDK.ElectronicDisplayStabilization = false;
		Cardboard.SDK.AutoDriftCorrection = false;
		Cardboard.SDK.EnableSettingsButton = true;
		VrModeChanged (Prefs.VrMode);
	}

	void OnEnable ()
	{
		Debug.Log ("***CardboardAssistantController.OnEnable()");
		Prefs.OnVrModeChanged += VrModeChanged;
	}

	void OnApplicationQuit ()
	{
		Debug.Log ("***CardboardAssistantController.OnApplicationQuit()");
		if (instanceCreated) {
			Cardboard.SDK.OnBackButton -= OnBackButton;
		}
		// assume Cardboard.SDK will be torn down
		instanceCreated = false;
	}

	void OnDestroy ()
	{
		Debug.Log ("***CardboardAssistantController.OnDestroy()");
	}

	void OnDisable ()
	{
		Debug.Log ("***CardboardAssistantController.OnDisable()");
		Prefs.OnVrModeChanged -= VrModeChanged;
	}

	public void VrModeChanged (bool vrMode)
	{
		Assert.IsTrue (instanceCreated);
		if (Cardboard.SDK.VRModeEnabled == vrMode) {
			return;
		}
		Cardboard.SDK.VRModeEnabled = vrMode;
		Debug.Log ("***Cardboard.SDK.VRModeEnabled -> " + Cardboard.SDK.VRModeEnabled);
		Camera.main.GetComponent<StereoController> ().UpdateStereoValues ();
	}

	void OnBackButton ()
	{
		Debug.Log ("***BACK BUTTON!");
		Prefs.VrMode = !Prefs.VrMode;
	}

}
