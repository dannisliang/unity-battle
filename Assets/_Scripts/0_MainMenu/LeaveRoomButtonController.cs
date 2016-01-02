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

	void Start ()
	{
		UpdateInteractable (GameController.instance.roomSetupPercent);
	}

	void OnEnable ()
	{
		GameController.OnRoomConnectStatusChanged += UpdateInteractable;
	}

	void OnDisable ()
	{
		GameController.OnRoomConnectStatusChanged -= UpdateInteractable;
	}

	void UpdateInteractable (int percent)
	{
		button.interactable = percent > 0;
	}

	public void LeaveRoom ()
	{
		Debug.Log ("***Leaving room …");
		GameController.gamesPlatform.RealTime.LeaveRoom ();
	}

}
