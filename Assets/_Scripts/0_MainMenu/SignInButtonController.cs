using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignInButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			GameController.instance.SignIn (false);
		});
	}

	void Start ()
	{
		GameController.instance.OnConnectStatusChanged += UpdateActive;
		GameController.instance.InvokeConnectStatusAction (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			GameController.instance.OnConnectStatusChanged -= UpdateActive;
		}
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (!authenticated);
	}

}
