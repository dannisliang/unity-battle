using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneMaster : MonoBehaviour
{
	public static SceneMaster instance { get; private set; }

	public const string SCENE_MAIN_MENU = "__MainMenu";
	public const string SCENE_GAME = "__Game";

	public static bool sawMainMenu { get; private set; }

	public static bool quitting { get; private set; }

	void Awake ()
	{
		if (SceneManager.GetActiveScene ().name.Equals (SCENE_MAIN_MENU)) {
			sawMainMenu = true;
		}	
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);

		if (sawMainMenu) {
			return;
		}
		LoadAsync (SCENE_MAIN_MENU);
	}

	void OnApplicationQuit ()
	{
		quitting = true;
	}

	public void LoadAsync (string menu)
	{
		Async (delegate {
			Debug.Log ("***Loading " + menu + " …");
			SceneManager.LoadScene (menu);
		});
	}

	public void Async (Action action, float delay = 0f)
	{
		StartCoroutine (Do (action, delay));
	}

	IEnumerator Do (Action action, float delay)
	{
		yield return new WaitForSeconds (delay);
		action ();
	}

}
