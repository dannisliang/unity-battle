using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class SignInButtonController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("***Clicked " + button.name);
			if (IsIteractable ()) {
				GameController.instance.Authenticate (false);
			}
		});
	}

	void Update ()
	{
		button.interactable = IsIteractable ();
	}

	bool IsIteractable ()
	{
		return !GameController.gamesPlatform.IsAuthenticated ();
	}

}
