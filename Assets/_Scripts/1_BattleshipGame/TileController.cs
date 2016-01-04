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
		eventData.Use ();
		Highlight (true);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		eventData.Use ();
		Highlight (false);
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		BattleshipController.instance.FireAt (eventData.pointerPressRaycast.gameObject.transform);
		eventData.Use ();
	}

	void Highlight (bool highlight)
	{
		BattleshipController.instance.AimAt (Whose.Theirs, highlight ? positionMarkerController.position : null);
#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.S)) {
			RealtimeBattleship.EncodeAndSend (positionMarkerController.position);
			BattleshipController.instance.Strike (Whose.Theirs, positionMarkerController.position);
		}
#endif
	}
}