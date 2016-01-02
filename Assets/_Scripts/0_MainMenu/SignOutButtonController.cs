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
			Debug.Log ("***Clicked " + button.name);
			SignOut ();
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
		button.interactable = authenticated;
	}

	void SignOut ()
	{
		Debug.Log ("***SignOut() …");
		GameController.instance.SignOut ();
	}

}
