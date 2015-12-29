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
		return PlayGamesPlatform.Instance.IsAuthenticated () && !PlayGamesPlatform.Instance.RealTime.IsRoomConnected ();
	}

	public void CreateQuickGameRoom ()
	{
		Debug.Log ("***Creating quick game room …");
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame (minOpponents: 1, maxOpponents : 1, variant : 0, listener: GameController.instance);
	}

	public void CreateWithInvitationScreenRoom ()
	{
		Debug.Log ("***Creating with invitation room …");
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen (minOpponents: 1, maxOppponents : 1, variant : 0, listener: GameController.instance);
	}

}
