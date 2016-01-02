using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomPanelController : MonoBehaviour
{

	void Start ()
	{
		GameController.instance.InvokeConnectStatusAction (UpdateActive);
		GameController.OnConnectStatusChanged += UpdateActive;
	}

	void OnDestroy ()
	{
		GameController.OnConnectStatusChanged -= UpdateActive;
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated);
	}

}
