using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomPanelController : MonoBehaviour
{

	void Start ()
	{
		Game.instance.InvokeConnectStatusAction (UpdateActive);
		Game.instance.OnConnectStatusChanged += UpdateActive;
	}

	void OnDestroy ()
	{
		Game.instance.OnConnectStatusChanged -= UpdateActive;
	}

	void UpdateActive (bool authenticated, bool isRoomConnected, int roomSetupPercent)
	{
		gameObject.SetActive (authenticated);
	}

}
