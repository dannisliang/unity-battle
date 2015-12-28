using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class SignInController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("Clicked " + button.name);
			if (IsIteractable ()) {
				Authenticate (false);
			}
		});
	}

	void Update ()
	{
		button.interactable = IsIteractable ();
	}

	bool IsIteractable ()
	{
		return !PlayGamesPlatform.Instance.IsAuthenticated ();
	}

	void Authenticate (bool silent)
	{
		Debug.Log ("Authenticate() …");
		PlayGamesPlatform.Instance.Authenticate ((bool success) => {
			Debug.logger.Log ("Authenticate --> " + (success ? "SUCCESS" : "FAILURE"));
		}, silent);
	}
}
