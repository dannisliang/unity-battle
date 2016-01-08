using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour
{
	public static GameController instance { get; private set; }




	AudioSource source;
	//	bool showingWaitingRoom;


	void Awake ()
	{
		if (!SceneMaster.sawMainMenu) {
			Destroy (gameObject);
			return;
		}
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	#if UNITY_EDITOR
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S)) {
			int scale = 2;
			string filename = PlayerSettings.bundleIdentifier + "-" + (Screen.width * scale) + "x" + (Screen.height * scale) + "-" + DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss") + ".png";
			Application.CaptureScreenshot (filename, scale);
			Debug.Log (scale + "x screenshot saved as " + filename);
		}
	}
	#endif

}
