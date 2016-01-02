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
			CreateQuickGameRoom ();
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
		button.interactable = authenticated && roomSetupPercent == 0;
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
