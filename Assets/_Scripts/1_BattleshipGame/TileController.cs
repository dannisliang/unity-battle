using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TileController : MonoBehaviour, IPointerClickHandler
{
	PositionMarkerController positionMarkerController;

	void Awake ()
	{
		positionMarkerController = GetComponent<PositionMarkerController> ();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		if (!BattleshipController.instance.IsFiring ()) {
			FireAt (eventData.pointerPress.transform);
		}
	}

	void FireAt (Transform targetTransform)
	{
		GameObject rocket = Instantiate (BattleshipController.instance.rocketPrefab);
		//rocket.transform.SetParent (transform, false);
		rocket.GetComponent<RocketController> ().MaybeLaunch (Camera.main.transform, targetTransform);
	}

}
