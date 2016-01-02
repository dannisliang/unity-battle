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
		button.interactable = percent == 0;
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
