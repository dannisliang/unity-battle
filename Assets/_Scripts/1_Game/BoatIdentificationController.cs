using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent (typeof(PositionMarkerController))]
public class BoatIdentificationController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	PositionMarkerController positionMarkerController;
	BoatController boatController;

	void Awake ()
	{
		positionMarkerController = gameObject.GetComponent<PositionMarkerController> ();
		boatController = gameObject.GetComponentInParent<BoatController> ();
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		Highlight (true);
		RealtimeBattle.EncodeAndSendAim (positionMarkerController.position);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		Highlight (false);
	}

	void Highlight (bool highlight)
	{
		boatController.Identify (highlight, positionMarkerController.position);
	}

}
