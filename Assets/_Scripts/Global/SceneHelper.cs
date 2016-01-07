using UnityEngine;
using System.Collections;

public class SceneHelper : MonoBehaviour
{

	void Awake ()
	{
		if (!SceneMaster.sawMainMenu) {
			// avoid errors due to not properly initialized game
//			Debug.Log ("***Disabling " + gameObject);
			gameObject.SetActive (false);
			return;
		}
	}

}
