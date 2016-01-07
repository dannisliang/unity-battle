using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TileController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	PositionMarkerController positionMarkerController;

	bool firedUpon;

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
		if (!SceneMaster.quitting) {
			eventData.Use ();
			Highlight (false);
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (firedUpon) {
			BattleController.instance.FalseFire ();
			return;
		}
		eventData.Use ();
		if (BattleController.instance.FireAt (eventData.pointerPressRaycast.gameObject.transform)) {
			firedUpon = true;
		}
	}

	void Highlight (bool highlight)
	{
		BattleController.instance.AimAt (Whose.Theirs, highlight ? positionMarkerController.position : null);
#if UNITY_EDITOR
		if (!firedUpon && Input.GetKey (KeyCode.F)) {
			RealtimeBattle.EncodeAndSend (positionMarkerController.position);
			StrikeResult result = BattleController.instance.Strike (Whose.Theirs, positionMarkerController.position);
			switch (result) {
			case StrikeResult.IGNORED_ALREADY_MISSED:
			case StrikeResult.IGNORED_ALREADY_HIT:
			case StrikeResult.MISS:
			case StrikeResult.HIT_NOT_SUNK:
			case StrikeResult.HIT_AND_SUNK:
				firedUpon = true;
				break;
			default:
				throw new System.NotImplementedException ();
			}
		}
#endif
	}
}