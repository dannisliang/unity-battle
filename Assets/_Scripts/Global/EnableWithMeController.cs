using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EnableWithMeController : MonoBehaviour
{

	public GameObject[] enableWhenIAmActive;
	public GameObject[] disableWhenIAmActive;

	void OnEnable ()
	{
		SetAllActive (disableWhenIAmActive, false);
		SetAllActive (enableWhenIAmActive, true);
	}

	void OnDisable ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		SetAllActive (enableWhenIAmActive, false);
		SetAllActive (disableWhenIAmActive, true);
	}

	void SetAllActive (GameObject[] gameObjects, bool active)
	{
		foreach (GameObject go in gameObjects) {
			go.SetActive (active);
		}
	}

}
