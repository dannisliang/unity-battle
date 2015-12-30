using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TileController : MonoBehaviour, IPointerDownHandler
{
	PositionMarkerController positionMarkerController;

	void Awake ()
	{
		positionMarkerController = GetComponent<PositionMarkerController> ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (!BattleshipController.instance.IsFiring ()) {
			eventData.Use ();
			FireAt (eventData.pointerPressRaycast.gameObject.transform);
		}
	}

	void FireAt (Transform targetTransform)
	{
		GameObject rocket = Instantiate (BattleshipController.instance.rocketPrefab);
		//rocket.transform.SetParent (transform, false);
		rocket.GetComponent<RocketController> ().MaybeLaunch (Camera.main.transform, targetTransform);
	}

}
