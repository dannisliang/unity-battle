using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.Assertions;

public class SceneMaster : MonoBehaviour
{
	public static SceneMaster instance { get; private set; }

	public const string SCENE_GAME = "__Game";
	public static readonly string[] buildLevels = {
		"Assets/" + SCENE_GAME + ".unity",
	};

	static bool _quitting;
	static bool _hardRestarting;

	public static bool quitting { get { return _quitting || _hardRestarting; } }

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
	}

	void OnApplicationQuit ()
	{
		_quitting = true;
	}

	public static void HardRestart ()
	{
		_hardRestarting = true;
		Debug.Log ("***LoadScene(0) …");
		SceneManager.LoadScene (0);
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
