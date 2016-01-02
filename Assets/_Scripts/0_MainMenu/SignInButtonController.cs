using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class SignInButtonController : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			Debug.Log ("***Clicked " + button.name);
			GameController.instance.Authenticate (false);
		});
	}

	void Start ()
	{
		GameController.instance.InvokeConnectStatusAction (UpdateInteractable);
	}

	void OnEnable ()
	{
		GameController.OnConnectStatusChanged += UpdateInteractable;
	}

	void OnDisable ()
	{
		GameController.OnConnectStatusChanged -= UpdateInteractable;
	}

	void UpdateInteractable (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		button.interactable = !authenticated;
	}

}
