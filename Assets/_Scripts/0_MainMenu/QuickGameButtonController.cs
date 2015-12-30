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
		GameController.gamesPlatform.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: GameController.instance);
	}

	public void CreateWithInvitationScreenRoom ()
	{
		Debug.Log ("***Creating with invitation room …");
		GameController.gamesPlatform.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: GameController.instance);
	}

}
