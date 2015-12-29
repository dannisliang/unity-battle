using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class SignOutButtonController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("Clicked " + button.name);
			if (IsIteractable ()) {
				SignOut ();
			}
		});
	}

	void Update ()
	{
		button.interactable = IsIteractable ();
	}


	bool IsIteractable ()
	{
		return PlayGamesPlatform.Instance.IsAuthenticated ();
	}

	void SignOut ()
	{
		Debug.Log ("SignOut() …");
		PlayGamesPlatform.Instance.SignOut ();
	}

}
