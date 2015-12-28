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
			if (!PlayGamesPlatform.Instance.IsAuthenticated ()) {
				Debug.Log ("Authenticate() …");
				Authenticate ();
			}
		});
	}

	void Authenticate ()
	{
		PlayGamesPlatform.Instance.Authenticate ((bool success) => {
			Debug.logger.Log ("Authenticate --> " + (success ? "SUCCESS" : "FAILURE"));
			if (success) {
				GameController.instance.CreateMultiplayerRoom ();
			}
		});
	}
}
