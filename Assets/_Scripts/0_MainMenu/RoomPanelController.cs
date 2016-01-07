using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomPanelController : MonoBehaviour
{

	void Start ()
	{
		ButlerController.instance.InvokeConnectStatusAction (UpdateActive);
		ButlerController.instance.OnConnectStatusChanged += UpdateActive;
	}

	void OnDestroy ()
	{
		ButlerController.instance.OnConnectStatusChanged -= UpdateActive;
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated);
	}

}
