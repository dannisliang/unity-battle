using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class LeaveRoomButtonController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("Clicked " + button.name);
			if (IsIteractable ()) {
				LeaveRoom ();
			}
		});
	}

	void Update ()
	{
		button.interactable = IsIteractable ();
	}

	bool IsIteractable ()
	{
		return PlayGamesPlatform.Instance.IsAuthenticated () && PlayGamesPlatform.Instance.RealTime.IsRoomConnected ();
	}

	public void LeaveRoom ()
	{
		Debug.logger.Log ("Leaving room …");
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
	}

}
