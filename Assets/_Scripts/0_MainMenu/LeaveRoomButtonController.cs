using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaveRoomButtonController : MonoBehaviour
{

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			GameController.instance.QuitGame ();
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
		gameObject.SetActive (authenticated && roomSetupPercent > 0);
	}

}
