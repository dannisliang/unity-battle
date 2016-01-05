using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class MainMenuController : MonoBehaviour
{

	void Awake ()
	{
		if (Time.frameCount <= 1 && GameController.instance == null) {
			// load main game level
			SceneManager.LoadScene (Utils.SCENE_GAME);
			// prevent NPEs due to non-existent GameController
			gameObject.SetActive (false);
		}	
	}

}
