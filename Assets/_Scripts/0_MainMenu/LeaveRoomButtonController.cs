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
			Debug.Log ("***Clicked " + button.name);
			LeaveRoom ();
		});
	}

	void OnEnable ()
	{
		GameController.OnConnectStatusChanged += UpdateInteractable;
		GameController.instance.InvokeConnectStatusAction (UpdateInteractable);
	}

	void OnDisable ()
	{
		GameController.OnConnectStatusChanged -= UpdateInteractable;
	}

	void UpdateInteractable (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		button.interactable = authenticated && roomSetupPercent > 0;
	}

	public void LeaveRoom ()
	{
		Debug.Log ("***Leaving room …");
		GameController.gamesPlatform.RealTime.LeaveRoom ();
	}

}
