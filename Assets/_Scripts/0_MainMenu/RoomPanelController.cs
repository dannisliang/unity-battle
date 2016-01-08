using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomPanelController : MonoBehaviour
{

	void Start ()
	{
		GameController.instance.InvokeConnectStatusAction (UpdateActive);
		GameController.instance.OnConnectStatusChanged += UpdateActive;
	}

	void OnDestroy ()
	{
		GameController.instance.OnConnectStatusChanged -= UpdateActive;
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated);
	}

}
