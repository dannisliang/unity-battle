using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class QuickGameButtonController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("***Clicked " + button.name);
			if (IsIteractable ()) {
				CreateQuickGameRoom ();
			}
		});
	}

	void Update ()
	{
		button.interactable = IsIteractable ();
	}

	bool IsIteractable ()
	{
//		return GameController.gamesPlatform.IsAuthenticated () && !GameController.gamesPlatform.RealTime.IsRoomConnected ();
		return GameController.instance.RoomSetupPercent () == 0;
	}

	public void CreateQuickGameRoom ()
	{
		Debug.Log ("***Creating quick game room …");
		GameController.instance.SetupRoom (false);
	}

	public void CreateWithInvitationScreenRoom ()
	{
		Debug.Log ("***Creating with invitation room …");
		GameController.instance.SetupRoom (true);
	}

}
