using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class SignOutController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("Clicked " + button.name);
			if (PlayGamesPlatform.Instance.IsAuthenticated ()) {
				Debug.Log ("SignOut() …");
				PlayGamesPlatform.Instance.SignOut ();
			}
		});
	}

}
