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
		Debug.Log ("BOOM " + positionMarkerController.position);
	}
}
