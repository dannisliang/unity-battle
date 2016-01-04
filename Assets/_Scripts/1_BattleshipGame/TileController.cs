using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TileController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	PositionMarkerController positionMarkerController;

	void Awake ()
	{
		positionMarkerController = GetComponent<PositionMarkerController> ();
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (!BattleshipController.instance.IsFiring ()) {
			eventData.Use ();
			Highlight (true);
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		eventData.Use ();
		Highlight (false);
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

	void Highlight (bool highlight)
	{
		BattleshipController.instance.AimAt (Whose.Theirs, highlight ? positionMarkerController.position : null);
#if UNITY_EDITOR
		if (highlight) {
			BattleshipController.instance.Strike (Whose.Theirs, positionMarkerController.position);
		}
#endif
	}
}